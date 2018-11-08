import {
    START_FETCH_LOGIN,
    FETCH_SUCCESS_LOGIN,
    FETCH_ERROR_LOGIN
} from '../actions/types';

const defaultState = {
    dataRes: null,
    isLoading: false,
    error: false,
    errorMessage: null
}

export default (state = defaultState, action) => {

    switch (action.type) {
        case START_FETCH_LOGIN:
            return {
                ...state,
                error: false,
                isLoading: true
            };
        case FETCH_SUCCESS_LOGIN:
            return {
                error: false,
                isLoading: false,
                dataRes: action.dataRes
            };
        case FETCH_ERROR_LOGIN:
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