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
    return {'hand': json.loads(user.state)['hand']}

@app.route('/users/<id>/state', methods=['POST'])
def write_user_state(id):
    user = User.query.get(id)
    user.state = json.dumps(request.json)
    db.session.commit()
    return {'username': user.username, 'newState': json.loads(user.state)}

### User friends view ###

@app.route('/users/<id>/friends')
def get_user_friends(id):
    user =  User.query.get_or_404(id)
    if user.friends is None:
        return {}
    return {'friends': json.loads(user.friends)}

@app.route('/users/<id>/friends', methods=['POST'])
def write_user_friends(id):
    user = User.query.get(id)
    user.friends = json.dumps(request.json['friends'])
    db.session.commit()
    return {'friends': json.loads(user.friends)}