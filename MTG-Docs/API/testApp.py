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

    def __repr__(self):
        return f"ID:{self.id} - {self.username}"

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
        return {'hand': [], 'deck': [], 'grave': [], 'exile': [], 'creatures': [], 'lands': [], 'others': [], 'hash': ''}
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
