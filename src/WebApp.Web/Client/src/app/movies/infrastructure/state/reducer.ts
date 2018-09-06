import { MovieAction } from './action';
import * as fromRoot from '../../../state/app.state';
import { StudioPage } from '../../../models/view/studio-page';
import { createFeatureSelector, createSelector } from '@ngrx/store';
import { Studio } from '../../../models/view';

export interface State extends fromRoot.State {
    movies: MovieState;
}

export interface MovieState {
    movies: StudioPage;
    currentStudio: Studio;
}

const getMoviesFeatureState = createFeatureSelector<MovieState>('movies');

export const getMoviesPage = createSelector(
    getMoviesFeatureState,
    state => state.movies
);

export const getCurrentStudio = createSelector(
    getMoviesFeatureState,
    state => state.currentStudio
);

// const initialState: MovieState = {
//     movies: {
//         currentPage: 1,
//         data: [],
//         pagesCount: 0,
//         pageSize: 0,
//         studio: null,
//         total: 0
//     }
// };

const initialState: MovieState = {
    movies: null,
    currentStudio: null
};

export function reducer(state = initialState, action): MovieState {
    switch (action.type) {
        case MovieAction.SET_MOVIES:
            return {
                ...state,
                movies: action.payload
            };
        case MovieAction.SET_STUDIO:
            return {
                ...state,
                currentStudio: action.payload
            };
        default:
            return state;
    }
}
