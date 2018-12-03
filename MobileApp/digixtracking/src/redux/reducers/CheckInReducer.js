import {
    START_FETCH_CHECKIN,
    FETCH_SUCCESS_CHECKIN,
    FETCH_ERROR_CHECKIN
} from '../actions/types';

const defaultState = {
    dataRes: null,
    isLoading: false,
    error: false,
    errorMessage: null
}

export default (state = defaultState, action) => {

    switch (action.type) {
        case START_FETCH_CHECKIN:
            return {
                ...state,
                error: false,
                isLoading: true
            };
        case FETCH_SUCCESS_CHECKIN:
            return {
                error: false,
                isLoading: false,
                dataRes: action.dataRes
            };
        case FETCH_ERROR_CHECKIN:
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