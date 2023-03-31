import json
from flask import Flask, request, redirect
from flask_sqlalchemy import SQLAlchemy
app = Flask(__name__)

app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///data.db'
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
db = SQLAlchemy(app)

app.app_context().push()

class Card(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(80), unique=True, nullable=False)
    description = db.Column(db.String(120))

    def __repr__(self):
        return f"{self.name} - {self.description}"

class User(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    username = db.Column(db.String(80), unique=True, nullable=False)
    state = db.Column(db.Text)
    password = db.Column(db.Text)
    friends = db.Column(db.Text)
    challenges = db.Column(db.Text)
    decks = db.Column(db.Text)
    draftPacks = db.Column(db.Text)
    draftQueue = db.Column(db.Text)
    activeDeck = db.Column(db.Text)

    def __repr__(self):
        return f"ID:{self.id} - {self.username}"


class Global(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(80), unique=True, nullable=False)
    content = db.Column(db.Text)

    def __repr__(self):
        return f"ID:{self.id} - {self.name}"

class Draft(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    hostId = db.Column(db.Integer, nullable=False)
    hostName = db.Column(db.String(80))
    capacity = db.Column(db.Integer)
    set1 = db.Column(db.String(80))
    set2 = db.Column(db.String(80))
    set3 = db.Column(db.String(80))
    setName = db.Column(db.String(80))
    players = db.Column(db.Text)
    started = db.Column(db.Integer)

    def __repr__(self):
        return f"ID:{self.id} - HostId:{self.hostId}"

class Lobby(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    hostId = db.Column(db.Integer, nullable=False)
    hostName = db.Column(db.String(80))
    players = db.Column(db.Text)
    started = db.Column(db.Integer)

    def __repr__(self):
        return f"ID:{self.id} - HostId:{self.hostId}"


@app.route('/')
def index():
    return redirect("pythonAPI/index.html", code=302)

@app.route('/cards')
def get_cards():
    cards = Card.query.all()

    output = []
    for card in cards:
        card_data = {'name': card.name, 'description': card.description}
        output.append(card_data)

    return {"cards" : output}

@app.route('/cards/<id>')
def get_card(id):
    card = Card.query.get_or_404(id)
    return {"name": card.name, "description": card.description}

@app.route('/cards', methods=['POST'])
def add_card():
    card = Card(name=request.json['name'], description=request.json['description'])
    db.session.add(card)
    db.session.commit()
    return {'id': card.id}

@app.route('/cards/<id>', methods=['DELETE'])
def delete_card(id):
    card = Card.query.get(id)
    if card is None:
        return {"error": "card not found"}
    db.session.delete(card)
    db.session.commit()
    return {"Successful deletion": id}

### All users view ###

@app.route('/users')
def get_users():
    users = User.query.all()

    output = []
    for user in users:
        user_data = {'id': user.id, 'username': user.username, 'password': user.password}
        output.append(user_data)

    return {"users" : output}

@app.route('/users', methods=['POST'])
def add_user():
    user = User(username=request.json['username'], password=request.json['password'])
    db.session.add(user)
    db.session.commit()
    return {'New user id': user.id, 'Username': user.username, 'Password': user.password}

### Individual user view ###

@app.route('/users/<id>')
def get_user(id):
    user = User.query.get_or_404(id)
    return {"id": user.id, "username": user.username, "password": user.password}

@app.route('/users/<id>', methods=['DELETE'])
def delete_user(id):
    user = User.query.get(id)
    if user is None:
        return {"Error": "User not found"}
    db.session.delete(user)
    db.session.commit()
    return {"Successful user deletion": id}

@app.route('/users/<id>/name')
def get_username(id):
    user = User.query.get_or_404(id)
    return {"name": user.username}

### Board state ###

@app.route('/users/<id>/state')
def get_user_state(id):
    user =  User.query.get_or_404(id)
    if user.state is None:
        return {'hand': [], 'deck': [], 'grave': [], 'exile': [], 'creatures': [], 'lands': [], 'others': [], 'life': 0, 'hash': '', 'coinToss': 0, 'tossTime': ''}
    return json.loads(user.state)

@app.route('/users/<id>/state', methods=['POST'])
def write_user_state(id):
    user = User.query.get(id)
    user.state = json.dumps(request.json)
    db.session.commit()
    return {'username': user.username, 'newState': json.loads(user.state)}

@app.route('/users/<id>/state', methods=['DELETE'])
def delete_user_state(id):
    user = User.query.get(id)
    user.state = None
    db.session.commit()
    return {'Success': 'State successfully deleted.'}

@app.route('/users/<id>/state/hash')
def get_user_state_hash(id):
    user =  User.query.get_or_404(id)
    if user.state is None:
        return ''
    return json.loads(user.state)['hash']

### User friends view ###

@app.route('/users/<id>/friends')
def get_user_friends(id):
    user =  User.query.get_or_404(id)
    if user.friends is None:
        return {'friends': []}
    return {'friends': json.loads(user.friends)}

@app.route('/users/<id>/friends', methods=['POST'])
def write_user_friends(id):
    user = User.query.get(id)
    user.friends = json.dumps(request.json['friends'])
    db.session.commit()
    return {'friends': json.loads(user.friends)}

### User decks view ###

@app.route('/users/<id>/decks')
def get_user_decks(id):
    user =  User.query.get_or_404(id)
    if user.decks is None:
        return {'decks': []}
    return {'decks': json.loads(user.decks)}

@app.route('/users/<id>/decks', methods=['POST'])
def write_user_decks(id):
    user = User.query.get(id)
    user.decks = json.dumps(request.json['decks'])
    db.session.commit()
    return {'decks': json.loads(user.decks)}

### User active deck view ###

@app.route('/users/<id>/activeDeck')
def get_user_activeDeck(id):
    user =  User.query.get_or_404(id)
    if user.activeDeck is None:
        return {'name': '', 'cards': [], 'sideboard': [], 'cardFrequencies': [], 'sideboardFrequencies': [], 'coverId': '', 'isDraft': False}
    return json.loads(user.activeDeck)

@app.route('/users/<id>/activeDeck', methods=['POST'])
def write_user_activeDeck(id):
    user = User.query.get(id)
    user.activeDeck = json.dumps(request.json)
    db.session.commit()
    return json.loads(user.activeDeck)

### User challenges view ###

@app.route('/users/<id>/challenges')
def get_user_challenges(id):
    user =  User.query.get_or_404(id)
    if user.challenges is None:
        return {'challenges': []}
    return {'challenges': json.loads(user.challenges)}

@app.route('/users/<id>/challenges', methods=['POST'])
def write_user_challenges(id):
    user = User.query.get(id)
    user.challenges = json.dumps(request.json['challenges'])
    db.session.commit()
    return {'challenges': json.loads(user.challenges)}

@app.route('/users/<id>/challenges', methods=['DELETE'])
def delete_user_challenges(id):
    user = User.query.get(id)
    user.challenges = "[]"
    db.session.commit()
    return {'challenges': json.loads(user.challenges)}

@app.route('/users/<id>/challenges/<chId>')
def get_user_challenge(id, chId):
    user =  User.query.get_or_404(id)
    challenges = json.loads(user.challenges)
    for challenge in challenges:
        for key, value in challenge.items():
            if key == 'challengerID' and str(value) == chId:
                return challenge
    return {}

@app.route('/users/<id>/challenges/<chId>', methods=['POST'])
def write_user_challenge(id, chId):
    user = User.query.get(id)
    incomingChallenge = request.json
    incomingValue = None
    for key, value in incomingChallenge.items():
        if key == 'accepted':
            incomingValue = value
            break
    allChallenges = json.loads(user.challenges if user.challenges != None else "[]")
    challengeFound = False
    for challenge in allChallenges:
        for key, value in challenge.items():
            if key == 'challengerID' and str(value) == chId:
                challenge['accepted'] = incomingValue
                challengeFound = True
                break
        if challengeFound:
            break
    if not challengeFound:
        allChallenges.append(incomingChallenge)
    user.challenges = json.dumps(allChallenges)
    db.session.commit()
    return incomingChallenge

@app.route('/users/<id>/challenges/<chId>', methods=['DELETE'])
def delete_user_challenge(id, chId):
    user = User.query.get(id)
    allChallenges = json.loads(user.challenges if user.challenges != None else "[]")
    challengeFound = False
    for challenge in allChallenges[:]:
        for key, value in challenge.items():
            if key == 'challengerID' and str(value) == chId:
                challengeFound = True
                break
        if challengeFound:
            allChallenges.remove(challenge)
            break

    user.challenges = json.dumps(allChallenges)
    db.session.commit()
    return {"Success": "Challenge deleted successfully"}

### Player draft packs view ###

@app.route('/users/<id>/draftPacks')
def get_user_draftPacks(id):
    user =  User.query.get_or_404(id)
    if user.draftPacks is None:
        return {'draftPacks': []}
    return {'draftPacks': json.loads(user.draftPacks)}

@app.route('/users/<id>/draftPacks', methods=['POST'])
def write_user_draftPacks(id):
    user = User.query.get(id)
    user.draftPacks = json.dumps(request.json['draftPacks'])
    # Reset the player's draft queue
    if user.draftQueue is not None:
        user.draftQueue = json.dumps(json.loads("[]"))
    db.session.commit()
    return {'draftPacks': json.loads(user.draftPacks)}

@app.route('/users/<id>/draftPacks/<packNum>')
def get_user_draftPacks_individual(id, packNum):
    user =  User.query.get_or_404(id)
    packs = json.loads(user.draftPacks if user.draftPacks != None else "[]")
    if len(packs) >= (int(packNum) + 1):
        return packs[int(packNum)]
    return {'cards': []}

@app.route('/users/<id>/draftPacks/<packNum>', methods=['POST'])
def write_user_draftPacks_individual(id, packNum):
    user = User.query.get(id)
    packs = json.loads(user.draftPacks if user.draftPacks != None else "[]")
    if len(packs) >= (int(packNum) + 1):
        packs[int(packNum)] = {'cards': request.json['cards']}
        user.draftPacks = json.dumps(packs)
        db.session.commit()
        return packs[int(packNum)]
    return {'cards': []}

### Player draft queue view ###

@app.route('/users/<id>/draftQueue')
def get_user_draftQueue(id):
    user =  User.query.get_or_404(id)
    if user.draftQueue is None:
        return {'draftQueue': []}
    return {'draftQueue': json.loads(user.draftQueue)}

@app.route('/users/<id>/draftQueue', methods=['POST'])
def add_user_draftQueue(id):
    user = User.query.get(id)
    queue = json.loads(user.draftQueue if user.draftQueue != None else "[]")
    pack = request.json
    queue.append(pack)
    user.draftQueue = json.dumps(queue)
    db.session.commit()
    return {'draftQueue': json.loads(user.draftQueue)}

@app.route('/users/<id>/draftQueue', methods=['DELETE'])
def delete_user_draftQueue(id):
    user = User.query.get(id)
    queue = json.loads(user.draftQueue if user.draftQueue != None else "[]")
    if len(queue) > 0:
        pack = queue.pop(0)
        user.draftQueue = json.dumps(queue)
        db.session.commit()
        return pack
    return {'cards': []}

@app.route('/users/<id>/draftQueue/consume')
def consume_user_draftQueue(id):
    user = User.query.get(id)
    queue = json.loads(user.draftQueue if user.draftQueue != None else "[]")
    if len(queue) > 0:
        pack = queue.pop(0)
        user.draftQueue = json.dumps(queue)
        db.session.commit()
        return pack
    return {'cards': []}

### Server globals ###

@app.route('/globals')
def get_globals():
    globals = Global.query.all()

    output = []
    for entry in globals:
        global_data = {'id': entry.id, 'name': entry.name, 'content': json.loads(entry.content)}
        output.append(global_data)

    return {"globals" : output}

@app.route('/globals/starters')
def get_starters():
    starters = Global.query.get(1)
    return json.loads(starters.content)

@app.route('/globals/starters', methods=['POST'])
def write_starters():
    starters = Global.query.get(1)
    starters.content = json.dumps(request.json)
    return json.loads(starters.content)

@app.route('/globals/proFeatured')
def get_proFeatured():
    proFeatured = Global.query.get(2)
    return json.loads(proFeatured.content)

@app.route('/globals/proFeatured', methods=['POST'])
def write_proFeatured():
    proFeatured = Global.query.get(2)
    proFeatured.content = json.dumps(request.json)
    return json.loads(proFeatured.content)

## All draft view ##

@app.route('/drafts')
def get_drafts():
    drafts = Draft.query.all()

    output = []
    for draft in drafts:
        draft_data = {'id': draft.id, 'hostId': draft.hostId, 'hostName': draft.hostName, 'capacity': draft.capacity, 'set1': draft.set1, 'set2': draft.set2, 'set3': draft.set3, 'setName': draft.setName, 'players': json.loads(draft.players), 'started': draft.started}
        output.append(draft_data)

    return {"drafts" : output}

@app.route('/drafts', methods=['POST'])
def add_draft():
    # Check if it already exists for this host, if so -> delete previous
    prevDraft = Draft.query.filter_by(hostId=request.json['hostId']).first()
    if prevDraft is not None:
        db.session.delete(prevDraft)
        db.session.commit()
    # Add new draft
    draft = Draft(hostId=request.json['hostId'], hostName=request.json['hostName'], capacity=request.json['capacity'], set1=request.json['set1'], set2=request.json['set2'], set3=request.json['set3'], setName=request.json['setName'], players=json.dumps(request.json['players']), started=request.json['started'])
    db.session.add(draft)
    db.session.commit()
    return {'New draft id': draft.id, 'Host': draft.hostId, 'HostName': draft.hostName, 'Capacity': draft.capacity, 'Set1': draft.set1, 'Set2': draft.set2, 'Set3': draft.set3, 'SetName': draft.setName, 'Players': json.loads(draft.players), 'Started': draft.started}

## Individual draft view ##

@app.route('/drafts/<hostID>', methods=['GET'])
def get_draft(hostID):
    draft = Draft.query.filter_by(hostId=hostID).first()
    if draft is None:
        return {"Error": "Draft not found"}
    return {'id': draft.id, 'hostId': draft.hostId, 'hostName': draft.hostName, 'capacity': draft.capacity, 'set1': draft.set1, 'set2': draft.set2, 'set3': draft.set3, 'setName': draft.setName, 'players': json.loads(draft.players), 'started': draft.started}

@app.route('/drafts/<hostID>', methods=['DELETE'])
def delete_draft(hostID):
    draft = Draft.query.filter_by(hostId=hostID).first()
    if draft is None:
        return {"Error": "Draft not found"}
    db.session.delete(draft)
    db.session.commit()
    return {"Successful draft deletion": hostID}

@app.route('/drafts/<hostID>/start', methods=['POST'])
def start_draft(hostID):
    draft = Draft.query.filter_by(hostId=hostID).first()
    if draft is None:
        return {"Error": "Draft not found"}
    draft.started = 1
    db.session.commit()
    return {"Successfully started draft": hostID}

## Individual draft player list ##

@app.route('/drafts/<hostID>/players', methods=['GET'])
def get_draft_players(hostID):
    draft = Draft.query.filter_by(hostId=hostID).first()
    if draft is None:
        return {"Error": "Draft not found"}
    return {'players': json.loads(draft.players)}

@app.route('/drafts/<hostID>/players/<playerID>', methods=['POST'])
def add_draft_player(hostID, playerID):
    draft = Draft.query.filter_by(hostId=hostID).first()
    if draft is None:
        return {"Error": "Draft not found"}
    players = json.loads(draft.players)
    if len(players) >= int(draft.capacity):
        return {"Error": "Draft is already full"}
    if int(playerID) not in players:
        players.append(int(playerID))
        draft.players = json.dumps(players)
        db.session.commit()
    return {'players': json.loads(draft.players)}

@app.route('/drafts/<hostID>/players/<playerID>', methods=['DELETE'])
def remove_draft_player(hostID, playerID):
    draft = Draft.query.filter_by(hostId=hostID).first()
    if draft is None:
        return {"Error": "Draft not found"}
    players = json.loads(draft.players)
    if int(playerID) in players:
        players.remove(int(playerID))
        draft.players = json.dumps(players)
        db.session.commit()
    return {'players': json.loads(draft.players)}

## All lobbies view ##

@app.route('/lobbies')
def get_lobbies():
    lobbies = Lobby.query.all()

    output = []
    for lobby in lobbies:
        lobby_data = {'id': lobby.id, 'hostId': lobby.hostId, 'hostName': lobby.hostName, 'players': json.loads(lobby.players), 'started': lobby.started}
        output.append(lobby_data)

    return {"lobbies" : output}

@app.route('/lobbies', methods=['POST'])
def add_lobby():
    # Check if it already exists for this host, if so -> delete previous
    prevLobby = Lobby.query.filter_by(hostId=request.json['hostId']).first()
    if prevLobby is not None:
        db.session.delete(prevLobby)
        db.session.commit()
    # Add new lobby
    lobby = Lobby(hostId=request.json['hostId'], hostName=request.json['hostName'], players=json.dumps(request.json['players']), started=request.json['started'])
    db.session.add(lobby)
    db.session.commit()
    return {'New lobby id': lobby.id, 'Host': lobby.hostId, 'HostName': lobby.hostName, 'Players': json.loads(lobby.players), 'Started': lobby.started}

## Individual lobby view ##

@app.route('/lobbies/<hostID>', methods=['GET'])
def get_lobby(hostID):
    lobby = Lobby.query.filter_by(hostId=hostID).first()
    if lobby is None:
        return {"Error": "Lobby not found"}
    return {'id': lobby.id, 'hostId': lobby.hostId, 'hostName': lobby.hostName, 'players': json.loads(lobby.players), 'started': lobby.started}

@app.route('/lobbies/<hostID>', methods=['DELETE'])
def delete_lobby(hostID):
    lobby = Lobby.query.filter_by(hostId=hostID).first()
    if lobby is None:
        return {"Error": "Lobby not found"}
    db.session.delete(lobby)
    db.session.commit()
    return {"Successful lobby deletion": hostID}

@app.route('/lobbies/<hostID>/start', methods=['POST'])
def start_lobby(hostID):
    lobby = Lobby.query.filter_by(hostId=hostID).first()
    if lobby is None:
        return {"Error": "Lobby not found"}
    lobby.started = 1
    db.session.commit()
    return {"Successfully started lobby": hostID}

## Individual lobby player list ##

@app.route('/lobbies/<hostID>/players', methods=['GET'])
def get_lobby_players(hostID):
    lobby = Lobby.query.filter_by(hostId=hostID).first()
    if lobby is None:
        return {"Error": "Lobby not found"}
    return {'players': json.loads(lobby.players)}

@app.route('/lobbies/<hostID>/players/<playerID>', methods=['POST'])
def add_lobby_player(hostID, playerID):
    lobby = Lobby.query.filter_by(hostId=hostID).first()
    if lobby is None:
        return {"Error": "Lobby not found"}
    players = json.loads(lobby.players)
    if len(players) >= 4:
        return {"Error": "Lobby is already full"}
    if int(playerID) not in players:
        players.append(int(playerID))
        lobby.players = json.dumps(players)
        db.session.commit()
    return {'players': json.loads(lobby.players)}

@app.route('/lobbies/<hostID>/players/<playerID>', methods=['DELETE'])
def remove_lobby_player(hostID, playerID):
    lobby = Lobby.query.filter_by(hostId=hostID).first()
    if lobby is None:
        return {"Error": "Lobby not found"}
    players = json.loads(lobby.players)
    if int(playerID) in players:
        players.remove(int(playerID))
        lobby.players = json.dumps(players)
        db.session.commit()
    return {'players': json.loads(lobby.players)}
