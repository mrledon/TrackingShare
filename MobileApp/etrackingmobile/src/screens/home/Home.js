import React, { Component } from 'react';
import { View, StyleSheet, TouchableOpacity, Alert, AsyncStorage } from 'react-native';
import { Text, Item } from 'native-base';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";

class Home extends Component {
  constructor(props) {
    super(props);
    this.state = {
    };
  }

  handleCheckIn = () => {
    this.props.navigation.navigate('CheckIn');
  }

  handleCheckOut = () => {
    this.props.navigation.navigate('CheckOut');
  }

  handleReport = () => {
    this.props.navigation.navigate('POSMDetail');
  }

  handleStoreList = async () => {
    this.props.navigation.navigate('StoreList');
  }

  handleStoreListLocal = async () => {
    this.props.navigation.navigate('StoreListLocal');
  }

  _storeData = async () => {
    try {
      await AsyncStorage.removeItem('USER_SSC');
      await AsyncStorage.removeItem('PASSWORD_SSC');

    } catch (error) {
    }
  }

  handleLogout = async () => {
    this._storeData();
    this.props.navigation.navigate('Login');
  }

  render() {
    return (
      <View
        style={styles.container}>
        <MainHeader
          hasLeft={false}
          title={STRINGS.HomeTitle} />
        <View
          padder
          style={styles.subContainer}>
          <MainButton
            style={styles.button}
            title={STRINGS.HomeCheckIn}
            onPress={() => this.handleCheckIn()} />
          <MainButton
            style={styles.button}
            title={STRINGS.HomeCheckOut}
            onPress={() => this.handleCheckOut()} />
          <MainButton
            style={styles.button}
            title={STRINGS.HomeCheckReport}
            onPress={() => this.handleReport()} />
          <MainButton
            style={styles.button}
            title={STRINGS.HomeCheckStoreListLocal}
            onPress={() => this.handleStoreListLocal()} />
          <MainButton
            style={styles.button}
            title={STRINGS.HomeCheckStoreList}
            onPress={() => this.handleStoreList()} />
        </View>
        <View style={styles.bottomContainer}>
          <TouchableOpacity onPress={() => this.handleLogout()}>
            <Text style={styles.textBottom}>{'Đăng xuất'}</Text>
          </TouchableOpacity>
        </View>
      </View>
    );
  }
}

const styles = StyleSheet.create({
  container: {
    flex: 1
  },
  subContainer: {
    flex: 1,
    flexDirection: 'column',
    alignItems: 'center',
    padding: 20
  },
  bottomContainer: {
    flexDirection: 'column',
    justifyContent: 'center',
    alignContent: 'center',
    paddingBottom: 50,
  },
  button: {
    height: 50
  },
  logo: {
    marginBottom: 20,
    width: 100,
    height: 100
  },
  textBottom: {
    textAlign: 'center',
    fontFamily: FONTS.MAIN_FONT_REGULAR,
  }
});

function mapStateToProps(state) {
  return {
  }
}

function dispatchToProps(dispatch) {
  return bindActionCreators({
  }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(Home);
