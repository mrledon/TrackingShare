import React, { Component } from 'react';
import { View, StyleSheet, Image, TouchableOpacity, Alert, AsyncStorage } from 'react-native';
import { Text, Item } from 'native-base';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import {
  fetchPushDataToServer
} from '../../redux/actions/ActionPushDataToServer';

const LOGO = require('../../assets/images/tracking.png');

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
    // this.props.navigation.navigate('StoreList');

    try {
      await AsyncStorage.removeItem('DATA_SSC');
      // await AsyncStorage.removeItem('PASSWORD_SSC');

    } catch (error) {
    }
  }

  handleStoreListLocal = async () => {
    this.props.navigation.navigate('StoreListLocal');

    // const _data = await AsyncStorage.getItem('DATA_SSC');
    // if (_data != null) {
    //   //Alert.alert('dataa'+_data.Id);
    //   let newProduct = JSON.parse(_data);
    //   // if (!newProduct) {
    //   Alert.alert(newProduct.length + '');
    //   console.log('datane', newProduct);

    //   if (newProduct.length != 0) {
    //     newProduct.forEach(element => {
    //       this.props.fetchPushDataToServer(element.Id, element.Code,
    //         element.Date, element.MasterStoreId, element.Token, element.TrackSessionId, element.PosmNumber, element.Photo)
    //         .then(() => setTimeout(() => {
    //           this.hello();
    //         }, 100));
    //     });
    //   }

    // }
    // else {
    //   Alert.alert('ko dataa');
    // }
  }

  hello = () => {
    const { PUSHdataRes, PUSHerror, PUSHerrorMessage } = this.props;

    if (PUSHerror) {
      let _mess = PUSHerrorMessage + '';
      if (PUSHerrorMessage == 'TypeError: Network request failed')
        _mess = STRINGS.MessageNetworkError;

      Alert.alert(
        STRINGS.MessageTitleError, _mess,
        [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
      );
      return;
    }
    else {
      if (PUSHdataRes.HasError == true) {
        Alert.alert(
          STRINGS.MessageTitleError, PUSHdataRes.Message + '',
          [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
        );
      } else if (PUSHdataRes.HasError == false) {
        Alert.alert('Upload thanh cong');
      }
    }
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
          <Image
            style={styles.logo}
            source={LOGO} />
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
            <Text style={styles.textBottom}>{STRINGS.HomeActionLogout}</Text>
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
    maxWidth: 500,
    padding: 20
  },
  bottomContainer: {
    flexDirection: 'column',
    justifyContent: 'center',
    alignContent: 'center',
    paddingBottom: 50
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
    PUSHdataRes: state.pushDataToServerReducer.dataRes,
    PUSHerror: state.pushDataToServerReducer.error,
    PUSHerrorMessage: state.pushDataToServerReducer.errorMessage,
  }
}

function dispatchToProps(dispatch) {
  return bindActionCreators({
    fetchPushDataToServer
  }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(Home);
