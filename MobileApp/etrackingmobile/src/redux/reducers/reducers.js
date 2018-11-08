import { combineReducers } from 'redux';

import LoginReducer from './LoginReducer';

const reducers = combineReducers({
    loginReducer: LoginReducer,
});

export default reducers;