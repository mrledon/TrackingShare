import {
    START_FETCH_CHECKOUT,
    FETCH_SUCCESS_CHECKOUT,
    FETCH_ERROR_CHECKOUT
} from '../actions/types';

const defaultState = {
    dataRes: null,
    isLoading: false,
    error: false,
    errorMessage: null
}

export default (state = defaultState, action) => {

    switch (action.type) {
        case START_FETCH_CHECKOUT:
            return {
                ...state,
                error: false,
                isLoading: true
            };
        case FETCH_SUCCESS_CHECKOUT:
            return {
                error: false,
                isLoading: false,
                dataRes: action.dataRes
            };
        case FETCH_ERROR_CHECKOUT:
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