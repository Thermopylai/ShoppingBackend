# ShoppingBackend

ShoppingBackend on ostoslistan tuotteiden hallintaan tarkoitettu ASP.NET Core -rajapinta. Se tallentaa tuotteet SQL Server -tietokantaan ja tarjoaa toimintojen kokeiluun Swagger-käyttöliittymän.

## Tekniikat

- .NET 10 ja ASP.NET Core Web API
- Entity Framework Core ja SQL Server
- Swagger / OpenAPI

## Käyttöönotto

Tarvitset .NET 10 SDK:n ja SQL Server -tietokannan, johon sovelluksella on luku- ja kirjoitusoikeudet.

### 1. Luo tietokanta ja taulu

Luo SQL Serveriin esimerkiksi `ShoplistDB`-tietokanta ja suorita siinä seuraava SQL. Jos käytät olemassa olevaa tietokantaa, varmista, että se vastaa sovelluksen tietomallia. Projektissa ei ole tietokantamigraatioita eikä sovellus luo taulua käynnistyessään.

```sql
CREATE TABLE dbo.Shoplist (
    id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    item nvarchar(50) NOT NULL,
    amount int NOT NULL
);
```

### 2. Määritä tietokantayhteys

Sovellus lukee yhteysmerkkijonon asetuksesta `ConnectionStrings:ShoplistConnection`. Korvaa versionhallinnan asetuksissa oleva yhteys omalla paikallisella asetuksellasi. Esimerkiksi PowerShellissä nykyiselle istunnolle:

```powershell
$env:ConnectionStrings__ShoplistConnection = 'Server=localhost;Database=ShoplistDB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True'
```

Muokkaa palvelimen nimeä ja todennustapaa oman SQL Server -asennuksesi mukaan. Esimerkki käyttää Windows-todennusta ja luottaa palvelimen varmenteeseen paikallista kehitystä varten. Älä tallenna salasanoja tai muita salaisuuksia versionhallintaan.

### 3. Käynnistä sovellus

Suorita repositorion juurihakemistossa:

```powershell
dotnet restore .\ShoppingBackend\ShoppingBackend.csproj
dotnet run --project .\ShoppingBackend\ShoppingBackend.csproj --launch-profile https
```

Avaa Swagger osoitteessa [https://localhost:7213](https://localhost:7213). HTTPS-profiili avaa myös HTTP-osoitteen `http://localhost:5133`, josta sovellus ohjaa HTTPS:ään. Jos kehitysvarmenteeseen ei luoteta, sen voi asentaa komennolla `dotnet dev-certs https --trust`.

Vaihtoehtoisesti avaa `ShoppingBackend.slnx` Visual Studiossa ja käynnistä projekti `https`-profiililla. Varmista, että tietokantayhteys on määritetty myös Visual Studion käynnistämän prosessin ympäristöön.

## Rajapinta

Peruspolku on `/api/Shoplist`. Tuotteen JSON-muoto on esimerkiksi:

```json
{
  "id": 1,
  "item": "Maito",
  "amount": 2
}
```

`id` on kokonaislukutunniste, `item` tuotteen nimi (enintään 50 merkkiä) ja `amount` kokonaislukumäärä. Tietokanta luo tunnisteen uusille riveille.

| Metodi ja polku | Toiminto |
| --- | --- |
| `GET /api/Shoplist` | Hae kaikki rivit. |
| `GET /api/Shoplist/{id}` | Hae rivi tunnisteella; puuttuva rivi palauttaa 404. |
| `POST /api/Shoplist` | Lisää rivi; onnistuminen palauttaa 201 ja luodun rivin. |
| `PUT /api/Shoplist/{id}` | Päivitä rivi; polun ja JSON-rungon tunnisteiden on vastattava toisiaan. |
| `DELETE /api/Shoplist/{id}` | Poista rivi; puuttuva rivi palauttaa 404. |
| `POST /api/Shoplist/search` | Hae rivejä, joiden nimi sisältää JSON-rungon `item`-tekstin. |
| `GET /api/Shoplist/filter?minAmount=1&maxAmount=5` | Hae rivit määrärajat mukaan lukien. |
| `GET /api/Shoplist/sort?sortBy=Item` | Järjestä rivit mallin ominaisuuden, esimerkiksi `Id`, `Item` tai `Amount`, mukaan. |
| `GET /api/Shoplist/paginate?pageNumber=1&pageSize=10` | Hae sivu; sivunumerointi alkaa yhdestä. |

Esimerkki uuden tuotteen lisäämisestä PowerShellissä sovelluksen ollessa käynnissä:

```powershell
Invoke-RestMethod -Method Post -Uri 'https://localhost:7213/api/Shoplist' -ContentType 'application/json' -Body '{"item":"Maito","amount":2}'
```

Lisäksi rajapinnassa on `count`, `exists/{id}`, `distinct-items`, `group-by-item`, `max-amount`, `min-amount`, `average-amount` ja `sum-amount` yhteenvetoja varten. `first`, `last` ja `single/{id}` hakevat yksittäisiä rivejä. `first` ja sivutus eivät määritä järjestystä; `last` hakee suurimman tunnisteen rivin.

Nimellä hakemiseen ovat käytettävissä `contains/{item}`, `find/{item}`, `find-first/{item}`, `find-last/{item}`, `find-single/{item}`, `find-any/{item}`, `find-all/{item}`, `find-contains/{item}`, `find-starts-with/{item}` ja `find-ends-with/{item}`. `find-in/{items}` ja `find-not-in/{items}` käyttävät pilkuilla erotettuja nimiä. Määrävertailuja varten ovat `find-between/{minAmount}/{maxAmount}`, `find-not-between/{minAmount}/{maxAmount}` sekä `find-greater-than/{amount}`, `find-less-than/{amount}`, `find-greater-than-or-equal/{amount}`, `find-less-than-or-equal/{amount}`, `find-equal/{amount}` ja `find-not-equal/{amount}`. Nämä vertailupolut palauttavat totuusarvon. Myös `any` kertoo, onko rivejä, ja `all`, ovatko kaikkien rivien määrät suurempia kuin nolla.

Kaikki yllä luetellut lisäpolut käyttävät `GET`-metodia, ellei muuta ole mainittu. Täydellinen reittiluettelo ja pyyntöjen kokeilu löytyvät Swaggerista. `find-single/{item}` edellyttää, että samalla nimellä löytyy enintään yksi rivi; tyhjään listaan kohdistuvat minimi-, maksimi- ja keskiarvokyselyt eivät palauta arvoa normaalisti.

## Huomioitavaa

Rajapintaan ei ole määritetty käyttäjien todennusta tai käyttöoikeusrajoituksia, ja CORS sallii kaikki alkuperät. Sovellus on suojattava asianmukaisesti ennen julkista käyttöä. Sovelluksen oletuskulttuuri on `fi-FI`.
