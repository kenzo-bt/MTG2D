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

    def __repr__(self):
        return f"ID:{self.id} - {self.username}"

class Global(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(80), unique=True, nullable=False)
    content = db.Column(db.Text)

    def __repr__(self):
        return f"ID:{self.id} - {self.name}"

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
