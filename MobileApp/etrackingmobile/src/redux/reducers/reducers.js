import { combineReducers } from 'redux';

import LoginReducer from './LoginReducer';
import CheckInReducer from './CheckInReducer';
import POSMDetailReducer from './POSMDetailReducer';

const reducers = combineReducers({
    loginReducer: LoginReducer,
    checkInReducer: CheckInReducer,
    POSMDetailReducer: POSMDetailReducer,
});

export default reducers;