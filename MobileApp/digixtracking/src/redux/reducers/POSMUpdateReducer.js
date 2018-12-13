import {
    START_FETCH_POSM_UPDATE,
    FETCH_SUCCESS_POSM_UPDATE,
    FETCH_ERROR_POSM_UPDATE
} from '../actions/types';

const defaultState = {
    dataRes: null,
    isLoading: false,
    error: false,
    errorMessage: null
}

export default (state = defaultState, action) => {

    switch (action.type) {
        case START_FETCH_POSM_UPDATE:
            return {
                ...state,
                error: false,
                isLoading: true
            };
        case FETCH_SUCCESS_POSM_UPDATE:
            return {
                error: false,
                isLoading: false,
                dataRes: action.dataRes
            };
        case FETCH_ERROR_POSM_UPDATE:
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