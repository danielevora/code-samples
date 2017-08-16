import { fetch, addTask } from 'domain-task';
import { Action, Reducer } from 'redux';
import { AppThunkAction } from './';

export interface CatImageCollectorState {
    catImageCollection: CatImage[];
}

export interface CatImage {
    image_id: string,
    src: string;
}

interface IncrementCatAction { type: 'INCREMENT_CAT' }
interface DecrementCatAction { type: 'DECREMENT_CAT' }
interface ReceiveCatAction {
    type: 'RECEIVE_CAT',
    catImage: CatImage
}
interface SaveCatsAction { type: 'SAVE_CATS' }
interface RestoreCatsAction { type: 'RESTORE_CATS' }

type KnownAction = IncrementCatAction | DecrementCatAction | ReceiveCatAction | SaveCatsAction | RestoreCatsAction;

export const actionCreators = {
    incrementCat: (): AppThunkAction<KnownAction> => (dispatch, getState) => { 
        let fetchTask = fetch('http://thecatapi.com/api/images/get?size=small&category=4')
                        .then(response => response.url as Promise<string>)
                        .then(data => {
                            dispatch({ type: 'RECEIVE_CAT', catImage: { src: data }})
                        });

        addTask(fetchTask);
    },
    decrementCat: () => <DecrementCatAction>{ type: 'DECREMENT_CAT' },
    saveCats: () => <SaveCatsAction>{ type: 'SAVE_CATS'}
};

export const reducer: Reducer<CatImageCollectorState> = (state: CatImageCollectorState, action: KnownAction) => {
    switch (action.type) {
        case 'RECEIVE_CAT':
            return { catImageCollection: [ action.catImage ] };
        default:
            const exhaustiveCheck: never = action;
    }

    return state || { catImageCollection: [] };
};
