import os
import json
import requests
import time

setImageDirectory = "./Images/Sets/LEA/"

if not os.path.exists(setImageDirectory):
    os.makedirs(setImageDirectory)

if not os.path.exists(setImageDirectory + "Northern Paladin.jpg"):
    # Download and save the image using the scryfall API
    response_API = requests.get('https://api.scryfall.com/cards/6303233b-35eb-49ca-b844-ba6b9fe1cbd2')
    time.sleep(10)
    data = response_API.text
    parse_json = json.loads(data)
    imageUrl = parse_json["image_uris"]["large"]

    image = requests.get(imageUrl)
    file = open(setImageDirectory + "Northern Paladin.jpg", "wb")
    file.write(image.content)
    file.close()
else:
    print("Image already exists!")
