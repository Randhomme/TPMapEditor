# TPMapEditor

## Get started
Download the [latest release](https://github.com/Randhomme/TPMapEditor/releases/latest) (TPMapEditor.zip). On the first opening, the app will ask you to set the TPGame path. Select your TPGame folder (where the game is located), and enjoy map editing !

Don't forget to read the wiki : https://github.com/Randhomme/TPMapEditor/wiki

Found a bug or have a suggestion ? Contact Randhomme on Discord.

## How to update the editor
If a new version is available, download it, extract it, and replace everything in your previous `TPMapEditor` folder (you should fuse the folders, this will keep your `TPMapEditor.xml` settings file, as well as your custom world objects images).

## Tips
- You can zoom in and out using `CTRL` + `Mouse wheel`
- You can scroll the map horizontally by using `Shift` + `Mouse wheel`
- Something on the map blocks the view ? You can modify it's Z index using +/- button on the top left of the editor, or simply hide it using the right tab (checkbox in the list)
- When rotation is enabled, use `ALT` + `Mouse wheel` to rotate selected elements. You can also rotate the preview of an element using `ALT` + `Mouse wheel` (even if rotation is not enabled)
- You can give color to `Groups`, `WaypointPath`, `WorldPolygon` and `WorldPointSet` to make them easier to see/recognize (colors are not used in game)
- If you don't find the string you are looking for, you can edit which header file is used for what in App>Settings>Edit game headers
- There is a guide on how to add world object images in the wiki

## In the future
Things you should expect in next updates :
- [ ] Write a complete documentation (values in the map file, values in the map editor)
- [ ] Undo/Redo feature (work in progress)
- [ ] Make the world bigger (5 times should be enough)
- [ ] Add support for saved game (technically done, should be improved)
- [ ] Add more app settings
  - [ ] Custom background (image or color)
  - [ ] Dark theme
  - [ ] Custom keyboard shortcut
  - [ ] Hide specific inactive layers
- [ ] Make the WorldRule editor smart, aware of the timeline
- [ ] Make it 3D (not now)
