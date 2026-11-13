# MusicPortal Admin Web API

The API uses the existing cookie authentication. Every endpoint below is restricted to users with the `Admin` role. Sign in through the MVC login page using an administrator account, then send the `MusicPortal.Auth` cookie with API requests. Requests that are not authenticated receive HTTP 401; authenticated non-admin users receive HTTP 403.

Base URL examples below assume the application is running locally at `https://localhost:7000`.

## Users — `/api/users`

| Method | Endpoint | Description | Success |
|---|---|---|---|
| GET | `/api/users` | Get all users (password hashes and salts are never returned) | 200 |
| GET | `/api/users/{id}` | Get one user | 200 / 404 |
| POST | `/api/users` | Create a user; the account is created unapproved and not as an administrator | 201 |
| PUT | `/api/users/{id}` | Update username, email, admin role and confirmation state | 204 |
| PATCH | `/api/users/{id}/approval` | Approve or reject/revoke approval | 204 |
| DELETE | `/api/users/{id}` | Delete a user (the current administrator cannot delete their own account) | 204 |

Example POST body:

```json
{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "StrongPassword123"
}
```

Example approval body:

```json
{
  "isRegistrationConfirmed": true
}
```

Set `isRegistrationConfirmed` to `false` to reject a pending registration or revoke an existing approval.

## Artists — `/api/artists`

| Method | Endpoint | Description | Success |
|---|---|---|---|
| GET | `/api/artists` | Get all artists | 200 |
| GET | `/api/artists/{id}` | Get one artist | 200 / 404 |
| POST | `/api/artists` | Create an artist | 201 |
| PUT | `/api/artists/{id}` | Update an artist | 204 |
| DELETE | `/api/artists/{id}` | Delete an artist if no records are linked | 204 / 409 |

Example body:

```json
{
  "name": "Artist name",
  "biography": "Short biography",
  "country": "Ukraine",
  "photoUrl": null
}
```

## Genres — `/api/genres`

| Method | Endpoint | Description | Success |
|---|---|---|---|
| GET | `/api/genres` | Get all genres | 200 |
| GET | `/api/genres/{id}` | Get one genre | 200 / 404 |
| POST | `/api/genres` | Create a genre | 201 |
| PUT | `/api/genres/{id}` | Update a genre | 204 |
| DELETE | `/api/genres/{id}` | Delete a genre if no records are linked | 204 / 409 |

Example body:

```json
{
  "name": "Jazz",
  "description": "Jazz music"
}
```

## Common HTTP status codes

- `200 OK` — a resource or collection was returned.
- `201 Created` — a resource was created; the response includes its representation and `Location` header.
- `204 No Content` — an update, approval change or deletion succeeded.
- `400 Bad Request` — invalid request or mismatched route/body IDs.
- `401 Unauthorized` — the request has no valid login cookie.
- `403 Forbidden` — the signed-in user is not an administrator.
- `404 Not Found` — the requested resource does not exist.
- `409 Conflict` — a user already exists or an artist/genre cannot be deleted because records refer to it.

All DTO responses omit password hashes and salts.
