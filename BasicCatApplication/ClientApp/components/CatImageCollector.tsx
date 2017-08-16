import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import { ApplicationState }  from '../store';
import * as CatImageCollectorStore from '../store/CatImageCollector';

type CatImageCollectorProps =
    CatImageCollectorStore.CatImageCollectorState
    & typeof CatImageCollectorStore.actionCreators
    & RouteComponentProps<{}>;

class CatImageCollector extends React.Component<CatImageCollectorProps, {}> {
    public render() {
        const { catImageCollection } = this.props;
        const catImageCount = catImageCollection.length;
        return <div>
            <h1>Cat Collector</h1>

            <p>Can't have too many cats!</p>

            <p>Cat count: <strong>{ catImageCount }</strong></p>

            <button onClick={ () => { this.props.incrementCat() } }>Cat Me!</button>

            { this.renderCatImages() }
        </div>;
    }

    private renderCatImages() {
        return <div>
                    { this.props.catImageCollection.map(catImage => 
                        <img key={catImage.src} src={catImage.src} />
                    )}
                </div>;
    }
}

export default connect(
    (state: ApplicationState) => state.catImages,
    CatImageCollectorStore.actionCreators
)(CatImageCollector) as typeof CatImageCollector;