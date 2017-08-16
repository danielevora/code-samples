import * as React from 'react';
import { RouteComponentProps } from 'react-router-dom';

export default class Home extends React.Component<RouteComponentProps<{}>, {}> {
    public render() {
        return <div>
            <h1>Hello, World!</h1>
            <p>This app doesn't do much, but it's an ok base for starting a new project.</p>
        </div>;
    }
}
