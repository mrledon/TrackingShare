import React, { Component } from 'react';
import { View, Alert, StyleSheet } from "react-native";
import { Text } from 'native-base';
import Spinner from 'react-native-loading-spinner-overlay';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import { fetchDataCheckIn } from '../../redux/actions/ActionCheckIn';
var moment = require('moment');

class CheckIn extends Component {

  constructor(props) {
    super(props);
    this.state = {
      currentTime: '',
      isVisible: false,
      initialPosition: null,
    }
  }

  componentDidMount() {
    setInterval(() => {
      this.setState({
        currentTime: moment().format('HH:mm:ss')
      })
    }, 1000)

    navigator.geolocation.getCurrentPosition(
      (position) => {
        let initialPosition = JSON.stringify(position);
        var obj = JSON.parse(initialPosition);
        this.setState({ initialPosition: obj });
      },
      (error) => console.log(error.message),
      { enableHighAccuracy: true, timeout: 20000, maximumAge: 1000 }
    );
  }

  handleBack = () => {
    this.props.navigation.navigate('Home');
  }

  handleCheckIn = () => {
    const { dataLogin } = this.props;
    const { currentTime, initialPosition } = this.state;

    let _coor = (initialPosition ? initialPosition.coords.latitude : '')+';'+(initialPosition ? initialPosition.coords.longitude : '');

    // Call API
    this.props.fetchDataCheckIn(dataLogin.Data.Id, dataLogin.Data.Token, currentTime, _coor)
      .then(() => setTimeout(() => {
        this.showAlert()
      }, 100));
  }

  // Handle status & result
  showAlert = () => {
    const { dataRes, error, errorMessage } = this.props;

    if (error) {
      let _mess = errorMessage + '';
      if (errorMessage == 'TypeError: Network request failed')
        _mess = STRINGS.MessageNetworkError;

      Alert.alert(
        STRINGS.MessageTitleError, _mess,
        [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
      );
      return;
    }
    else {
      if (dataRes.HasError == true) {
        Alert.alert(
          STRINGS.MessageTitleError, dataRes.Message + '',
          [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
        );
      } else if (dataRes.HasError == false) {
        Alert.alert(
          STRINGS.MessageTitleAlert, 'Điểm danh thành công',
          [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
        );
      }
    }
  }

  render() {

    const { currentTime } = this.state;
    const { isLoading } = this.props;

    return (
      <View
        style={styles.container}>
        <Spinner visible={isLoading} />

        <MainHeader
          onPress={() => this.handleBack()}
          hasLeft={true}
          title={STRINGS.CheckInTitle} />
        <View
          padder
          style={styles.subContainer}>
          <Text>{STRINGS.CheckInSubTitle}</Text>
          <Text style={styles.text}>{currentTime}</Text>
          <MainButton
            style={styles.button}
            title={STRINGS.CheckInAction}
            onPress={() => this.handleCheckIn()} />
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
    justifyContent: 'center',
    maxWidth: 500,
    padding: 50
  },
  text: {
    marginTop: 20,
    marginBottom: 20,
    fontSize: 20,
    color: 'red',
    textAlign: 'center',
    fontFamily: FONTS.MAIN_FONT_BOLD
  }
})

function mapStateToProps(state) {
  return {
    dataLogin: state.loginReducer.dataRes,

    isLoading: state.checkInReducer.isLoading,
    dataRes: state.checkInReducer.dataRes,
    error: state.checkInReducer.error,
    errorMessage: state.checkInReducer.errorMessage
  }
}

function dispatchToProps(dispatch) {
  return bindActionCreators({
    fetchDataCheckIn
  }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(CheckIn);