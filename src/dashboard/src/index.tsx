import React, { FC, useEffect, useState } from "react";
import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";
import ReactDOM from "react-dom";
import { ListenerTrack, CurrentTrack } from "./models";

import noMusic from "./no-music.jpg";
import "./index.css";

const baseUrl = process.env.REACT_APP_BASE_URL;

const Image: FC<{ track?: CurrentTrack }> = ({ track }) => {
  if (!track) {
    return <img src={noMusic} alt="No album cover" />;
  }
  return <img src={track.item.album.images[0].url} alt={track.item.name} />;
};

const TrackTitle: FC<{ track?: CurrentTrack }> = ({ track }) => {
  if (!track) {
    return <h2>No active track playing</h2>;
  }
  const suffix = !track.is_playing ? "[Paused]" : "";
  const artists = track.item.artists.map(i => i.name).join(", ");
  return (
    <h2>
      {artists} – {track.item.name} {suffix}
    </h2>
  );
};

const PlayLink: FC<{ track?: CurrentTrack }> = ({ track }) => {
  if (!track) {
    return null;
  }

  return (
    <a href={track.item.uri} className="playLink">
      <svg viewBox="0 0 300 300">
        <g transform="translate(100, 100)">
          <path d="M75.8059949,48.8718386 L39.3264031,23.7598295 C38.8173469,23.4116097 38.1607143,23.3731046 37.6168367,23.6627298 C37.0746173,23.9523549 36.7346939,24.5215605 36.7346939,25.1426642 L36.7346939,75.3666824 C36.7346939,75.9877861 37.0746173,76.5569916 37.6184949,76.8466168 C37.8605867,76.9771992 38.127551,77.0408163 38.3928571,77.0408163 C38.7195153,77.0408163 39.0461735,76.9420424 39.3264031,76.749517 L75.8059949,51.6375079 C76.2586735,51.326119 76.5306122,50.8088116 76.5306122,50.2546733 C76.5306122,49.7005349 76.2586735,49.1832275 75.8059949,48.8718386 Z M41.9901148,69.5280537 L41.9901148,30.9812928 L69.9901148,50.2546733 L41.9901148,69.5280537 Z" />
          <path d="M50,0 C22.43,0 0,22.43 0,50 C0,77.57 22.43,100 50,100 C77.57,100 100,77.57 100,50 C100,22.43 77.57,0 50,0 Z M50,95 C25.1873214,95 5,74.8126786 5,50 C5,25.1873214 25.1873214,5 50,5 C74.8126786,5 95,25.1873214 95,50 C95,74.8126786 74.8126786,95 50,95 Z" />
        </g>
      </svg>
    </a>
  );
};

const Listener: FC<{ listener: ListenerTrack }> = ({ listener }) => {
  const inactive = !listener.currentTrack || !listener.currentTrack.is_playing;
  const className = inactive ? "inactive" : "";

  return (
    <figure className={className}>
      <Image track={listener.currentTrack} />

      <figcaption>
        <header>
          <h1>{listener.userId}</h1>
          <TrackTitle track={listener.currentTrack} />
        </header>
        <PlayLink track={listener.currentTrack} />
      </figcaption>
    </figure>
  );
};

const NowPlayingGrid: FC<{ listeners: Array<ListenerTrack> }> = ({
  listeners
}) => {
  return (
    <main>
      {listeners.map(l => (
        <Listener key={l.userId} listener={l} />
      ))}
    </main>
  );
};

function App() {
  const [listeners, setListeners] = useState<Array<ListenerTrack>>([]);
  useEffect(function() {
    async function wrap() {
      try {
        const result = await fetch(`${baseUrl}/api/listeners`);
        const listeners = await result.json();
        setListeners(listeners);
      } catch (_) {
        console.error("Could not fetch.");
      }
    }
    wrap();
  }, []);
  useEffect(function() {
    const connection = new HubConnectionBuilder()
      .withUrl(`${baseUrl}/api`)
      .configureLogging(LogLevel.Information)
      .build();

    connection.start();
    connection.on("updateTracks", setListeners);
    return voidify(connection.stop);
  }, []);

  return <NowPlayingGrid listeners={listeners} />;
}

ReactDOM.render(<App />, document.getElementById("root"));

function voidify<T>(i: () => T) {
  return () => {
    i();
  };
}
