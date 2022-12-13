# MTG2D
MTG simulator in 2D made in Unity

## Current features

- Deck Browsing / Building / Editing / Saving

## Future features

- Analog game mechanics and interacting with the battlefield
- Networked play

## How the data was gathered

- The python script `getCardsFromSet.py` inside `MTG-Docs/CardDatabase` will parse through JSON set information downloaded from https://mtgjson.com/ and generate a reduced JSON file with only the necessary information.
- Additionally the script will also download the card images from the Scryfall API and store them in separate folders inside `MTG-Docs/CardDatabase/Images/Sets`

## Adding a new set

- To add a new set one would need to run the `getCardsFromSet.py` script with the set code as its only argument. E.g. `python getCardsFromSet.py USG`
- Then move the generated file E.g. `ReducedSets/USG.json` into the game's `Assets/Resources/Sets` folder.
- Edit the game's `Assets/Resources/ActiveSets.txt` file and add the new set.
- Upload the images to a remote server and configure the `PlayerManager.cs` variables (`serverUrl` / `serverImageFileExtension`) to fetch from it.
