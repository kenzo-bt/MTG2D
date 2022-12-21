from sqlalchemy import create_engine, MetaData, Table, Column, Integer, String
engine = create_engine('sqlite:///data.db', echo = True)
meta = MetaData()

user = Table(
   'user', meta,
   Column('id', Integer, primary_key = True),
   Column('username', String)
)
meta.create_all(engine)
