import {
    SAVE_POSM
} from '../actions/types';

const defaultState = {
    dataRes: null,
}

export default (state = defaultState, action) => {

    switch (action.type) {
        case SAVE_POSM:
            return {
                ...state,
                dataRes: action.posm
            };
        default:
            return state;
    }
}