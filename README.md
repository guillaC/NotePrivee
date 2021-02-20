# privateNote
Simple, clean self destructing note site

## Features
- [x] Self destructing notes
- [x] AES256 encryption
- [x] Administration Panel
- [x] API

## Screenshots
![screenshot]()
![screenshot]()

## Configuration
### Database
```SQL
CREATE TABLE `notes` (
  `id` int(11) NOT NULL,
  `contenu` text NOT NULL,
  `nombre_vue` int(11) NOT NULL,
  `date_creation` datetime NOT NULL DEFAULT current_timestamp(),
  `date_expiration` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `users` (
  `Id` int(11) NOT NULL,
  `Username` varchar(50) NOT NULL,
  `Password` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


INSERT INTO `users` (`Id`, `Username`, `Password`) VALUES(1, 'admin', 'd033e22ae348aeb5660fc2140aec35850c4da997'); -- admin/admin
ALTER TABLE `notes` ADD PRIMARY KEY (`id`);
ALTER TABLE `users` ADD PRIMARY KEY (`Id`);
ALTER TABLE `notes` MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=48;
ALTER TABLE `users`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;
COMMIT;
```

## API
### Create one note
#### Request
`POST /api/Notes`

`curl -X POST "https://localhost:5001/api/Notes" -H  "accept: application/json" -H  "Content-Type: application/json" -d "{  \"contenu\": \"hey\",  \"nombreVue\": 1,  \"dateExpiration\": \"2021-02-20T17:35:12.035Z\"}"`
```json
{
  "contenu": "hey",
  "nombreVue": 1,
  "dateExpiration": "2021-02-20T17:35:12.035Z"
}
```

#### Response
```json
{
  "id": "49",
  "key": "1eccea57eae34e7c8a418347da4d2a"
}
```

### Read one note
#### Request
`GET /api/Notes`

`curl -X GET "https://localhost:5001/api/Notes?id=49&key=1eccea57eae34e7c8a418347da4d2a" -H  "accept: text/plain"`
`https://localhost:5001/api/Notes?id=49&key=1eccea57eae34e7c8a418347da4d2a`
##### Parameters
* id - integer
* key - string
#### Response
```json
{
  "contenu": "hey",
  "nombreVue": 1,
  "dateExpiration": "2022-02-20T17:35:12"
}
```

## Tests
Tests are located in "NotePrivee.Test" directory. They are developed with XUnit Framework.

## Authors
[Guillaume](https://github.com/guillaC), [Gaëtan](https://github.com/), [Adrien](https://github.com/)

## Built With
- ASP Net Core 5.0
- [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) - Entity Framework Core provider for MySQL compatible databases.
- [ChartJSCore](https://github.com/mattosaurus/ChartJSCore) -  library for generating Chart.js code.
- [HtmlSanitizer](https://github.com/mganss/HtmlSanitizer) - For cleaning HTML fragments and documents from constructs that can lead to XSS attacks.
- [SimpleAES](https://github.com/jonjomckay/dotnet-simpleaes) - Wrapper to encrypt and decrypt using AES256
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) - Middleware to expose Swagger JSON endpoints from APIs built on ASP.NET Core
- [Westwind.AspNetCore.Markdown](https://github.com/RickStrahl/Westwind.AspNetCore.Markdown) - For Markdown support 
---
- [Bootstrap](https://getbootstrap.com/)
- [Jquery](https://jquery.com/)
- [Font-awesome](https://fontawesome.com/)
- [SimpleMDE](https://simplemde.com/)
- [ChartJS](https://www.chartjs.org/)
- [Highlight.js](https://highlightjs.org/)
