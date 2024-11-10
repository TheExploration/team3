# Contributing




## Command Line/Command Prompt/Cmd

Everyone should have team3 git repository cloned already. Open terminal and 

run `git clone https://github.com/TheExploration/team3.git`

1. Open the project folder 'team3'

2. Download the changes from the github repo (run everytime, before you work in Unity): Switch to the master branch with `git checkout master`. Then run: `git pull` > downloads all the changes from the github and updates everything locally. 

2. Create a new branch with `git branch YOUR_BRANCH` BEFORE YOU OPEN UNITY. NEVER REUSE BRANCHES ALWAYS MAKE NEW ONE FOR CHANGES!, Switch to your branch `git checkout YOUR_BRANCH`

4. Then you can code and make your changes!

5. To push your updated version to the Github, CLOSE UNITY FIRST, run `git checkout master` (switch branch), then run `git pull` (update the master branch if there were changes), and then switch back to your branch with `git checkout YOUR_BRANCH`


    1. then run `git add .`, then `git commit -m "update message"`, then `git merge master`, follow the instructions/resolve conflicts, then run `git push -u` and it should ask you to sign in to github. 



    2. Then send a message in the discord and I will make a pull request. Or make a pull request yourself with YOUR_BRANCH -> master. THEN WE DELETE THE BRANCH AFTER IT IS MERGED.

Merge conflicts for the scene file use always use --take-ours.

##### IF ANYONE RUNS INTO CONFLICTS WITH MERGING:
To reset your local files to match the online Github project, delete your project folder and run `git clone https://github.com/TheExploration/team3.git`

to generate a new project folder.

Test

## Github Desktop

...