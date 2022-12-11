# Utility script to read from a set's JSON file (downloaded from https://mtgjson.com/)
# Gets relevant set/card data only and produce a significantly smaller JSON file for better game performance
# Downloads the card images using the scryfall API

import os
import sys
import json
import requests
import time

# Verify that the script is run with correct arguments
if len(sys.argv) != 2:
    print("ERROR: Expected exactly 1 argument (Set code)")
    print("USAGE: python getCardsFromSet.py <setCode>")
    sys.exit()

# Define input/output paths for JSON files
fileType = ".json"
setReadDirectory = "./Sets/"
setWriteDirectory = "./ReducedSets/"
setCode = sys.argv[1]
setInputPath = setReadDirectory + setCode + fileType
setOutputPath = setWriteDirectory + setCode + fileType
setImageDirectory = "./Images/Sets/" + setCode + "/"

# If image directory doesnt exist, create it
if not os.path.exists(setImageDirectory):
    os.makedirs(setImageDirectory)

# Define output schema
outputSet = {
    "setCode": "",
    "setName": "",
    "cards": []
}

# Read in input JSON file
with open(setInputPath, encoding="utf8") as inputFile:
    setInformation = json.load(inputFile)["data"]

# Get set information
outputSet["setCode"] = setInformation["code"]
outputSet["setName"] = setInformation["name"]

# Get cards from set
cards = setInformation["cards"]
for card in cards:
    text = ""
    try:
        text = card["originalText"]
    except KeyError:
        for key, value in card.items():
            if key == "originalText":
                text = value

    thisCard = {
        "id": card["uuid"],
        "name": card["name"],
        "text": text,
        "colours": card["colors"],
        "convertedManaCost": card["convertedManaCost"],
        "types": card["types"],
        "rarity": card["rarity"]
    }
    outputSet["cards"].append(thisCard)

    if not os.path.exists(setImageDirectory + card["uuid"] + ".jpg"):
        # Get image URL from Scryfall API (https://scryfall.com/docs/api)
        # ! IMPORTANT ! Using sleep method to stay compliant with Scryfall API request limits (Max 10 request per second)
        scryfallResponse = requests.get('https://api.scryfall.com/cards/' + card["identifiers"]["scryfallId"])
        time.sleep(0.1)
        data = scryfallResponse.text
        parse_json = json.loads(data)
        imageUrl = parse_json["image_uris"]["large"]

        # Save image to the image output directory
        image = requests.get(imageUrl)
        file = open(setImageDirectory + card["uuid"] + ".jpg", "wb")
        file.write(image.content)
        file.close()

# Encode outputSet into JSON format and write to output JSON file
outputJSON = json.dumps(outputSet, indent=4)
with open(setOutputPath, "w") as outputFile:
    outputFile.write(outputJSON)
