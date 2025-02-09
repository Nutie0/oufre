Test bdd : http://localhost:5157/db-test (GET)
Inscription : http://localhost:5157/api/auth/register (POST)
                -Donne Json (body/raw) :
                                -{
                              "email": "nutierajaoniah@gmail.com",
                                "password": "mdp123"
                              } (Exemple ftsn io fa tsy maintsy mail Existant ftsn ) 

Validation par mail : http://localhost:5157/api/auth/verify?token=!TOKEN HAZO AVY MIPOTRA LIEN!&pin=!OTRZAY KO TY! (GET)

Login : http://localhost:5157/api/auth/login (POST):
{
    "email": "nutierajaoniah@gmail.com",
    "password": "mdp123"
}               
