import { combineReducers } from 'redux';

import LoginReducer from './LoginReducer';
import CheckInReducer from './CheckInReducer';
import POSMDetailReducer from './POSMDetailReducer';
import PushDataToServerReducer from './PushDataToServerReducer';
import PushInfoToServerReducer from './PushInfoToServerReducer';

const reducers = combineReducers({
    loginReducer: LoginReducer,
    checkInReducer: CheckInReducer,
    POSMDetailReducer: POSMDetailReducer,
    pushDataToServerReducer: PushDataToServerReducer,
    pushInfoToServerReducer: PushInfoToServerReducer
});

export default reducers;