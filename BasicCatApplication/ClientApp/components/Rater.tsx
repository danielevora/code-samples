import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import { ApplicationState }  from '../store';
//import * as CounterStore from '../store/Counter';

type RaterProps =
     RouteComponentProps<{}>;

class Rater extends React.Component<RaterProps, {}> {
    public render() {
        return <span>*****</span>;
    }
}

export default Rater as typeof Rater;