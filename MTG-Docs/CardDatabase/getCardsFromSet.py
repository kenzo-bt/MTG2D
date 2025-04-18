# Utility script to read from a set's JSON file (downloaded from https://mtgjson.com/)
# Gets relevant set/card data only and produce a significantly smaller JSON file for better game performance
# Optionally downloads the card images using the scryfall API

import os
import sys
import json
import requests
import time

# Verify that the script is run with correct arguments
if len(sys.argv) < 2 or len(sys.argv) > 3:
    print("ERROR: Invalid number of arguments")
    print("USAGE: python getCardsFromSet.py <setCode> [-p|t|u]")
    print("Use the -p flag to download the card images")
    print("Use the -t flag to download the token images")
    print("Use the -u flag to upload the card ids to the server")
    sys.exit()

# Define input/output paths for JSON files
fileType = ".json"
setReadDirectory = "./Sets/"
setWriteDirectory = "./ReducedSets/"
setCode = sys.argv[1]
setInputPath = setReadDirectory + setCode + fileType
setOutputPath = setWriteDirectory + setCode + fileType
setImageDirectory = "./Images/Sets/" + setCode + "/"
setTokenDirectory = "./Images/Sets/T" + setCode + "/"

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

# Gather all card ids in this set
cardIds = []

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

    manaCost = ""
    try:
        manaCost = card["manaCost"]
    except KeyError:
        manaCost = ""

    backside = ""
    isBack = False
    try:
        if card["side"] == "a":
            backside = card["otherFaceIds"][0]
        if card["side"] == "b":
            isBack = True
    except KeyError:
        backside = ""
        isBack = False

    layout = ""
    try:
        layout = card["layout"]
    except KeyError:
        backside = ""

    artist = ""
    try:
        artist = card["artist"]
    except KeyError:
        artist = ""

    variations = []
    try:
        variations = card["variations"]
    except KeyError:
        variations = []

    imageSize = "normal"
    imageUrl = ""
    scryfallId = card["identifiers"]["scryfallId"]
    if isBack:
        if layout == "meld":
            imageUrl = "https://cards.scryfall.io/" + imageSize + "/front/" + scryfallId[0] + "/" + scryfallId[1] + "/" + scryfallId + ".jpg?1"
        else:
            imageUrl = "https://cards.scryfall.io/" + imageSize + "/back/" + scryfallId[0] + "/" + scryfallId[1] + "/" + scryfallId + ".jpg?1"
    else:
        imageUrl = "https://cards.scryfall.io/" + imageSize + "/front/" + scryfallId[0] + "/" + scryfallId[1] + "/" + scryfallId + ".jpg?1"

    thisCard = {
        "id": card["uuid"],
        "imageUrl": imageUrl,
        "name": card["name"],
        "text": text,
        "colours": card["colors"],
        "colourIdentity": card["colorIdentity"],
        "manaValue": card["manaValue"],
        "manaCost": manaCost,
        "types": card["types"],
        "supertypes": card["supertypes"],
        "subtypes": card["subtypes"],
        "rarity": card["rarity"],
        "set": card["setCode"],
        "backId": backside,
        "isBack": isBack,
        "layout": layout,
        "isToken": False,
        "finishes": card["finishes"],
        "artist": artist,
        "language": card["language"],
        "variations": variations
    }
    outputSet["cards"].append(thisCard)

    if not isBack:
        cardIds.append(card["uuid"])

    if len(sys.argv) == 3 and sys.argv[2] == "-p":
        if not os.path.exists(setImageDirectory + card["uuid"] + ".jpg"):
            # Save image to the image output directory
            image = requests.get(imageUrl)
            file = open(setImageDirectory + card["uuid"] + ".jpg", "wb")
            file.write(image.content)
            file.close()

# Get tokens from set
tokens = setInformation["tokens"]
if len(tokens) > 0:
    print("Tokens found in this set...")
    # If token directory doesnt exist, create it
    if not os.path.exists(setTokenDirectory):
        os.makedirs(setTokenDirectory)
for token in tokens:
    artist = ""
    try:
        artist = token["artist"]
    except KeyError:
        artist = ""

    imageSize = "normal"
    imageUrl = ""
    scryfallId = token["identifiers"]["scryfallId"]
    if isBack:
        imageUrl = "https://cards.scryfall.io/" + imageSize + "/back/" + scryfallId[0] + "/" + scryfallId[1] + "/" + scryfallId + ".jpg?1"
    else:
        imageUrl = "https://cards.scryfall.io/" + imageSize + "/front/" + scryfallId[0] + "/" + scryfallId[1] + "/" + scryfallId + ".jpg?1"

    thisToken = {
        "id": token["uuid"],
        "imageUrl": imageUrl,
        "name": token["name"],
        "text": "",
        "colours": token["colors"],
        "colourIdentity": token["colorIdentity"],
        "manaValue": 0,
        "manaCost": "",
        "types": token["types"],
        "supertypes": token["supertypes"],
        "subtypes": token["subtypes"],
        "rarity": "common",
        "set": token["setCode"],
        "backId": "",
        "isBack": False,
        "layout": "",
        "isToken": True,
        "finishes": token["finishes"],
        "artist": artist,
        "language": token["language"],
        "variations": []
    }
    outputSet["cards"].append(thisToken)

    if len(sys.argv) == 3 and sys.argv[2] == "-t":
        if not os.path.exists(setTokenDirectory + token["uuid"] + ".jpg"):
            # Save image to the image output directory
            image = requests.get(imageUrl)
            file = open(setTokenDirectory + token["uuid"] + ".jpg", "wb")
            file.write(image.content)
            file.close()

# Encode outputSet into JSON format and write to output JSON file
outputJSON = json.dumps(outputSet, indent=4)
with open(setOutputPath, "w") as outputFile:
    outputFile.write(outputJSON)

# Send id list to API
if len(sys.argv) == 3 and sys.argv[2] == "-u":
    print("Sending to server...")
    # postUrl = 'https://mirariapi.onrender.com'
    postUrl = 'http://127.0.0.1:5000'
    postPayload = {'cardIds': cardIds}
    postResponse = requests.post(postUrl + '/cards', json=postPayload)
    print('Server response status code ' + str(postResponse.status_code))
    sys.exit()
