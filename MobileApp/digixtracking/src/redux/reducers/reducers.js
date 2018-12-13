import { combineReducers } from 'redux';

import LoginReducer from './LoginReducer';
import CheckInReducer from './CheckInReducer';
import CheckOutReducer from './CheckOutReducer';
import POSMDetailReducer from './POSMDetailReducer';
import PushDataToServerReducer from './PushDataToServerReducer';
import PushInfoToServerReducer from './PushInfoToServerReducer';
import DoneDataReducer from './DoneDataReducer';
import POSMReducer from './POSMReducer';
import POSMUpdateReducer from './POSMUpdateReducer';

const reducers = combineReducers({
    loginReducer: LoginReducer,
    checkInReducer: CheckInReducer,
    checkOutReducer: CheckOutReducer,
    POSMDetailReducer: POSMDetailReducer,
    pushDataToServerReducer: PushDataToServerReducer,
    pushInfoToServerReducer: PushInfoToServerReducer,
    doneDataReducer: DoneDataReducer,
    POSMReducer: POSMReducer,
    POSMUpdateReducer: POSMUpdateReducer
});

export default reducers;