export interface User {
  id: string;
  name: string;
  spotifyUri: string;
  spotifyHttpUrl: string;
}

export interface ListenerTrack {
  userId: string;
  currentTrack: CurrentTrack;
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
