import { combineReducers } from 'redux';

import LoginReducer from './LoginReducer';
import CheckInReducer from './CheckInReducer';
import POSMDetailReducer from './POSMDetailReducer';
import PushDataToServerReducer from './PushDataToServerReducer';

const reducers = combineReducers({
    loginReducer: LoginReducer,
    checkInReducer: CheckInReducer,
    POSMDetailReducer: POSMDetailReducer,
    pushDataToServerReducer: PushDataToServerReducer
});

export default reducers;