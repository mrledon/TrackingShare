import React, { Component } from "react";
import { StackNavigator } from "react-navigation";

import { Provider, connect } from "react-redux";
import { bindActionCreators } from "redux";
import store from "./redux/store/store.js";

import { Welcome } from './screens/welcome';
import { Home } from './screens/home';
import { Login } from './screens/account';
import { CheckIn } from './screens/checkin';
import { CheckOut } from './screens/checkout';
import { POSMDetail } from './screens/report';
import { StoreList, StoreListLocal } from './screens/store';
import { POSMStore, POSMOption, POSMTakePhoto } from './screens/posm';

const AppNavigator = StackNavigator(
  {
    Welcome: {
      screen: Welcome,
      navigationOptions: {
        gesturesEnabled: false,
      }
    },
    Login: {
      screen: Login,
      navigationOptions: {
        gesturesEnabled: false,
      }
    },
    Home: {
      screen: Home,
      navigationOptions: {
        gesturesEnabled: false,
      }
    },
    POSMDetail: {
      screen: POSMDetail,
      navigationOptions: {
        gesturesEnabled: false,
      }
    },
    POSMStore: {
      screen: POSMStore,
      navigationOptions: {
        gesturesEnabled: false,
      }
    },
    POSMOption: {
      screen: POSMOption,
      navigationOptions: {
        gesturesEnabled: false,
      }
    },
    POSMTakePhoto: {
      screen: POSMTakePhoto,
      navigationOptions: {
        gesturesEnabled: false,
      }
    },
    CheckIn: { screen: CheckIn },
    CheckOut: { screen: CheckOut },
    StoreList: { screen: StoreList },
    StoreListLocal: { screen: StoreListLocal },
  },
  {
    initialRouteName: "Welcome",
    headerMode: "none"
  }
);

class App extends Component {
  render() {
    return <AppNavigator />;
  }
}

const mapStateToProps = (state) => ({});

function dispatchToProps(dispatch) {
  return bindActionCreators({}, dispatch);
}

const AppWithNavigation = connect(
  mapStateToProps,
  dispatchToProps
)(App);

export default () => (
  <Provider store={store}>
    <AppWithNavigation />
  </Provider>
);