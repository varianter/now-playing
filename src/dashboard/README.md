# Now Playing – Dashboard

Dette er dashboardet til en app som skal vise aktiv sang fra Spotify for alle de som er autorisert til løsningen.
Målet med dashboardet er å ha en oversiktelig og visuell fremstilling av hva forskjellige folk hører på.

Kort oppsumert fungerer det slik:

1. En bruker autoriserer via OAuth mot Spotify.
2. Denne brukeren legges til en tabell i backend.
3. Vi kan spørre etter alle autoriserte brukere
4. Vi kan lytte på WebSockets (via SignalR) for lytterinformasjon

## Oppstart

Det er brukt Yarn for å installere avhengigheter og har derfor en `yarn.lock` fil. Avhengigheter kan derfor brukes med det.

```sh
# Fra root /src/dashboard
yarn install
```

Legg til `.env` fil basert på `.env.example` og fyll ut `REACT_APP_BASE_URL`.

Start løsningen med

```sh
yarn start
```

Rediger i `src/index.tsx` eller `src/index.js` alt etter.

## API

Her er en kort oppsummering av de endepunktene vi kan bruke:

`<baseUrl>` i eksemplene kan være `process.env.REACT_APP_BASE_URL` hvor `REACT_APP_BASE_URL` er satt i `.env` fil lokalt.

### `GET <baseUrl>/api/users -> Array<User>`

Henter ut liste av alle brukere. Modell:

```ts
export interface User {
  id: string;
  name: string;
  spotifyUri: string;
  spotifyHttpUrl: string;
}
```

### `GET <baseUrl>/api/authorize`

Brukes til å legge til en ny bruker til systemet. Redirecter tilbake til `http://localhost:3000`. Kan brukes som en vanlig lenke, uten AJAX.

### `POST <baseUrl>/api/negotiate`

Brukes for å koble til SignalR og WebSockets. Med bruk av SignalR-klient legges `/negotiate` til selv. Eksempelbruk:

```js
const connection = new HubConnectionBuilder()
  .withUrl(`${this.baseUrl}/api`)
  .configureLogging(LogLevel.Information)
  .build();

connection.on("updateTracks", (listenerTracks?: Array<ListenerTrack>) => {
  console.log(listenerTracks);
});
```

Hvor modeller ser slik ut:

```ts
export interface ListenerTrack {
  UserId: string;
  CurrentTrack: CurrentTrack;
}

export interface CurrentTrack {
  item: Track;
  is_playing: boolean;
  progress_ms: number;
}

export interface Track {
  name: string;
  uri: string;
  duration_ms: number;
  artists: Array<Artist>;
  album: Album;
}

export interface Artist {
  id: string;
  name: string;
  uri: string;
}

export interface Album {
  id: string;
  name: string;
  uri: string;
  images: Array<Image>;
}

export interface Image {
  height: number;
  url: string;
  width: number;
}
```

### `GET <baseUrl>/api/listeners -> Array<ListenerTrack>`

Henter ut alle brukere med hvilken sang de hører på akkurat nå. Returnerer liste av `ListenerTrack` som definert over.

**Note: burde ikke brukes med polling da det påvirker API rate limit hos Spotify**
