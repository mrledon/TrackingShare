import {
    START_FETCH_DONE_DATA,
    FETCH_SUCCESS_DONE_DATA,
    FETCH_ERROR_DONE_DATA
} from '../actions/types';

const defaultState = {
    dataRes: null,
    isLoading: false,
    error: false,
    errorMessage: null
}

export default (state = defaultState, action) => {

    switch (action.type) {
        case START_FETCH_DONE_DATA:
            return {
                ...state,
                error: false,
                isLoading: true
            };
        case FETCH_SUCCESS_DONE_DATA:
            return {
                error: false,
                isLoading: false,
                dataRes: action.dataRes
            };
        case FETCH_ERROR_DONE_DATA:
            return {
                ...state,
                error: true,
                isLoading: false,
                errorMessage: action.error
            };
        default:
            return state;
    }
}