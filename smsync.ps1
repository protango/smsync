param (
    [string]$TryBranch = "working"
)

function Take-Input {
    param (
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [string]$msg,
        [string]$ForegroundColor = "yellow"
    )

    Write-Host -ForegroundColor $ForegroundColor -NoNewline "$msg`: ";
    return Read-Host
}

function Get-Modified-Files {
    $gitStatus = git status -s;
    if (-not $gitStatus) {$gitStatus = ""}
    return [regex]::Matches($gitStatus, ".. `"?([^`"]+)`"?") | % {$_.Groups[1].Value}
}

function Is-Clean {
    $modifiedFiles = Get-Modified-Files
    $smList = $(git config --file .gitmodules --name-only --get-regexp path) -split "`r`n" -replace ".*\.(.*)\..*", "`$1"

    $allChangesAreSubmodules = $true
    foreach ($change in $modifiedFiles) {
        if ($smList -notcontains $change) {
            $allChangesAreSubmodules = $false
            break
        }
    }

    if ($modifiedFiles -and -not $allChangesAreSubmodules) {
        return $false
    } else {
        return $true
    }
}

function Clean-Working-Tree {
    if (-not $(Is-Clean)) {
        $folderName = Split-path -Leaf $(Get-Location).Path
        echo " "
        git status
        $response = Take-Input "Working tree for `"$folderName`" is unclean, (D)iscard changes, (S)tash, (C)ommit, or (I)gnore"
        if ($response -eq "D") {
            git reset head --hard
        } elseif ($response -eq "S") {
            git stash
        } elseif ($response -eq "C") {
            $commitMsg = Take-Input "Commit message"
            git add .
            git commit -m $commitMsg
        }
    }
}


function Update-Submodules {
    param (
        $path
    )
    if (-not $path) {echo "Error!"; return}
    # make path absolute
    $path = (resolve-path $path).Path
    $folderName = Split-path -Leaf $path
    cd $path

    $smList = $(git config --file .gitmodules --name-only --get-regexp path) -split "`r`n" -replace ".*\.(.*)\..*", "`$1"
    ForEach ($sm in $smList)
    {
        Update-Submodules $sm
    }

    # cd back into this dir since the recursive step above might've moved us
    cd $path

    $branches = $(git branch) -replace "[\* ] " -split "`r`n"
    $currentBranch = $(git branch --show-current)
    if ($branches -contains $TryBranch) {
        $targetBranch = $TryBranch
    } else {
        $targetBranch = "master"
    }

    Clean-Working-Tree
    if (-not (Is-Clean)) {
        echo "Aborting update on `"$folderName`" because working tree is unclean"
        return
    }

    if ($targetBranch -ne $currentBranch) {
        git checkout $targetBranch
    }
    $gitPull = git pull

    $modifiedFiles = Get-Modified-Files
    if ((Is-Clean)) {
        if ($modifiedFiles) {
            # We need to commit SM Update
            echo "`r`nThe following submodules have new commits:"
            write-host $modifiedFiles -ForegroundColor Red
            $response = Take-Input "Would you like to update their references in `"$folderName`" and commit (Y/N)"

            if ($response -eq "Y") {
                ForEach ($file in $modifiedFiles) {
                    git commit -m "Updated submodule '$file'" `"$file`"
                }
            }
        }
    } else {
        # Working dir should be clean from above, obviously something went wrong
        echo "ERROR processing repo: '$folderName'"
        return
    }

    if ("$(git status)" -like "*Your branch is ahead*") {
        $response = Take-Input "Unpushed changes for `"$folderName`" have been detected, would you like to push these (Y/N)"
        if ($response -eq "Y") {
            git push
        }
    }
    write-host "Completed `"$folderName`" ($targetBranch) - $(if ($gitPull -eq "Already up to date.") { $gitPull } else { "Updated to latest." })"  -ForegroundColor Green
}

$isGit = cmd /c 'git rev-parse --is-inside-work-tree 2>NUL'

if ($isGit -eq "true") {
    # Go to root git repo
    while ($gitSuperDir = git rev-parse --show-superproject-working-tree) {
        cd $gitSuperDir
    }

    Update-Submodules (Get-Location).Path
} else {
    write-error -Message "Must be in a git repository"
}