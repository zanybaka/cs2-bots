[Alternative solution](https://github.com/roflmuffin/CounterStrikeSharp/issues/372) (preferable):
1. [Download VPKEdit](https://github.com/craftablescience/VPKEdit/releases)
2. Open VPKEdit and open the VPK file .\game\csgo\pak01_000.vpk
3. Extract botprofile.db to an empty folder
4. Make the desired changes in the botprofile.db with a text editor
5. Using VPKEdit, create VPK from folder, select the folder you extracted to in step 3
6. Rename the vpk to botprofile.vpk
7. Copy the new VPK to .\game\csgo\overrides\botprofile.vpk (Create the overrides folder)
8. Modify .\game\csgo\gameinfo.gi file, add the bold line between lowViolence and Game csgo, under FileSystem -> SearchPaths

Game_LowViolence	csgo_lv
Game	csgo/overrides/botprofile.vpk
Game	csgo