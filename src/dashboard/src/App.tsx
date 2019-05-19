import React, { Component } from "react";
import "./App.css";
import { HubConnectionBuilder, LogLevel, HubConnection } from "@aspnet/signalr";
import { User, ListenerTrack } from "./models";

interface SignalRState {
  connection?: HubConnection;
  users?: Array<User>;
  listenerTracks?: Array<ListenerTrack>;
}

class App extends Component<{}, SignalRState> {
  state: SignalRState = {};
  baseUrl = process.env.REACT_APP_BASE_URL;

  async componentDidMount() {
    const connection = new HubConnectionBuilder()
      .withUrl(`${this.baseUrl}/api`)
      .configureLogging(LogLevel.Information)
      .build();

    await connection.start();

    this.setState({ connection });

    await fetch(`${this.baseUrl}/api/ping`);
    const result = await fetch(`${this.baseUrl}/api/users`);

    var users: Array<User> = await result.json();

    this.setState({ users });

    connection.on("updateTracks", (listenerTracks?: Array<ListenerTrack>) => {
      console.log(listenerTracks);
      this.setState({ listenerTracks });
    });
  }

  render() {
    const { users, listenerTracks } = this.state;

    return (
      <div className="App">
        <header className="App-header">
          <a href={`${this.baseUrl}/api/authorize`}>Authorize</a>
          {users ? (
            <ul>
              {users.map(u => (
                <li key={u.id}>{u.name}</li>
              ))}
            </ul>
          ) : null}
          {listenerTracks ? (
            <ul>
              {listenerTracks
                .filter(lt => lt.CurrentTrack != null)
                .map(lt => (
                  <li key={lt.UserId}>
                    {lt.UserId}: {lt.CurrentTrack.item.name} (
                    {lt.CurrentTrack.progress_ms} /{" "}
                    {lt.CurrentTrack.item.duration_ms}){" "}
                  </li>
                ))}
            </ul>
          ) : null}
        </header>
      </div>
    );
  }
}

export default App;
