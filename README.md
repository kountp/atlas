# Atlas Starter v0.1

Πρώτη λειτουργική βάση της Atlas.

## Περιλαμβάνει

- Visual Studio solution
- Atlas.Domain
- Atlas.Api
- Swagger
- Health check
- Πρώτη οντότητα Company
- Πρώτο endpoint δημιουργίας εταιρείας

## Τι χρειάζεται

- Visual Studio 2022 Community με το workload **ASP.NET and web development**
- .NET 8 SDK

## Εκτέλεση

1. Άνοιξε το `Atlas.sln`.
2. Όρισε το `Atlas.Api` ως Startup Project.
3. Πάτησε `F5`.
4. Θα ανοίξει το Swagger στο `http://localhost:5080/swagger`.

## Έλεγχος

- `GET /health`
- `GET /api`
- `POST /api/companies`

Παράδειγμα JSON:

```json
{
  "name": "IT Connect",
  "vatNumber": "123456789"
}
```

Σημείωση: Σε αυτή την έκδοση δεν υπάρχει ακόμη βάση δεδομένων. Το endpoint αποδεικνύει ότι το solution, το Domain και το API λειτουργούν σωστά.
