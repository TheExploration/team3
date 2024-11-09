# Contributing




## Command Line/Command Prompt/Cmd

Everyone should have team3 git repository cloned already. Open terminal and 

run `git clone https://github.com/TheExploration/team3.git`

1. Open the project folder 'team3' right click the blank area in the folder and press 'Open in Terminal'

2. Switch to the branch you are working on using `git checkout BRANCH_NAME` or create a new branch `git branch BRANCH_NAME` BEFORE YOU OPEN UNITY

2. To download the changes from the github repo (run everytime, before you work in Unity) Run: `git pull` > downloads all the changes from the github and updates everything locally. 


4. Then you can code and make your changes!

5. To push your updated version to the Github, CLOSE UNITY FIRST, run `git checkout master`, then run `git pull`, then `git checkout BRANCH_NAME`, 

then run `git add .`, then `git commit -m "update message"`, then `git merge master`, follow the instructions it gives you, then run `git push -u` and it should ask you to sign in to github, then the code will be uploaded online for everyone. 

Merge conflicts for the scene file use always use --take-ours.

##### IF ANYONE RUNS INTO CONFLICTS WITH MERGING:
To reset your local files to match the online Github project, delete your project folder and run `git clone https://github.com/TheExploration/team3.git`

to generate a new project folder.


## Github Desktop

...