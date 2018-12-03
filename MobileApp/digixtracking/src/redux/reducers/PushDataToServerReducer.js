import {
    START_FETCH_PUSH_DATA_TO_SERVER,
    FETCH_SUCCESS_PUSH_DATA_TO_SERVER,
    FETCH_ERROR_PUSH_DATA_TO_SERVER,
    IS_CHECK
} from '../actions/types';

const defaultState = {
    dataRes: null,
    isLoading: false,
    error: false,
    errorMessage: null,
    isCheck: false
}

export default (state = defaultState, action) => {

    switch (action.type) {
        case START_FETCH_PUSH_DATA_TO_SERVER:
            return {
                ...state,
                error: false,
                isLoading: true
            };
        case FETCH_SUCCESS_PUSH_DATA_TO_SERVER:
            return {
                error: false,
                isLoading: false,
                dataRes: action.dataRes
            };
        case FETCH_ERROR_PUSH_DATA_TO_SERVER:
            return {
                ...state,
                error: true,
                isLoading: false,
                errorMessage: action.error
            };
        case IS_CHECK:
            return {
                ...state,
                isCheck: action.isCheck
            };
        default:
            return state;
    }
}