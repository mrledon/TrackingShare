import {
    START_FETCH_STORE_TYPE,
    FETCH_SUCCESS_STORE_TYPE,
    FETCH_ERROR_STORE_TYPE,

    START_FETCH_DISTRICS,
    FETCH_ERROR_DISTRICS,
    FETCH_SUCCESS_DISTRICS,

    START_FETCH_PROVINCES,
    FETCH_ERROR_PROVINCES,
    FETCH_SUCCESS_PROVINCES,

    START_FETCH_WARDS,
    FETCH_ERROR_WARDS,
    FETCH_SUCCESS_WARDS,

    START_FETCH_STORE_BY_CODE,
    FETCH_ERROR_STORE_BY_CODE,
    FETCH_SUCCESS_STORE_BY_CODE,
} from '../actions/types';

const defaultState = {
    dataResListStoreType: null,
    dataResListProvinces: null,
    dataResListDistricts: null,
    dataResListWards: null,
    dataResStore: null,
    isLoading: false,
    error: false,
    errorMessage: null
}

export default (state = defaultState, action) => {

    switch (action.type) {
        case START_FETCH_STORE_TYPE:
            return {
                ...state,
                error: false,
                isLoading: true
            };
        case FETCH_SUCCESS_STORE_TYPE:
            return {
                error: false,
                isLoading: false,
                dataResListStoreType: action.dataRes
            };
        case FETCH_ERROR_STORE_TYPE:
            return {
                ...state,
                error: true,
                isLoading: false,
                errorMessage: action.error
            };

        case START_FETCH_PROVINCES:
            return {
                ...state,
                error: false,
                isLoading: true
            };
        case FETCH_SUCCESS_PROVINCES:
            return {
                error: false,
                isLoading: false,
                dataResListProvinces: action.dataRes
            };
        case FETCH_ERROR_PROVINCES:
            return {
                ...state,
                error: true,
                isLoading: false,
                errorMessage: action.error
            };

        case START_FETCH_DISTRICS:
            return {
                ...state,
                error: false,
                isLoading: true
            };
        case FETCH_SUCCESS_DISTRICS:
            return {
                error: false,
                isLoading: false,
                dataResListDistricts: action.dataRes
            };
        case FETCH_ERROR_DISTRICS:
            return {
                ...state,
                error: true,
                isLoading: false,
                errorMessage: action.error
            };

        case START_FETCH_WARDS:
            return {
                ...state,
                error: false,
                isLoading: true
            };
        case FETCH_SUCCESS_WARDS:
            return {
                error: false,
                isLoading: false,
                dataResListWards: action.dataRes
            };
        case FETCH_ERROR_WARDS:
            return {
                ...state,
                error: true,
                isLoading: false,
                errorMessage: action.error
            };

        case START_FETCH_STORE_BY_CODE:
            return {
                ...state,
                error: false,
                isLoading: true
            };
        case FETCH_SUCCESS_STORE_BY_CODE:
            return {
                error: false,
                isLoading: false,
                dataResStore: action.dataRes
            };
        case FETCH_ERROR_STORE_BY_CODE:
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