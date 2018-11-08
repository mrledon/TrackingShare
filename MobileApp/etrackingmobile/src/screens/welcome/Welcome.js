import React, { Component } from 'react';
import { View, AsyncStorage, ActivityIndicator, Alert, Dimensions, StyleSheet, Image } from "react-native";
import LinearGradient from 'react-native-linear-gradient';
import { Text } from 'native-base';
import { COLORS, FONTS, STRINGS } from '../../utils';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import { fetchDataLogin } from '../../redux/actions/ActionLogin';

const { height } = Dimensions.get('window');
const LOGO = require('../../assets/images/tracking.png');

class Welcome extends Component {

  constructor(props) {
    super(props);
  }

  componentDidMount() {
    this._retrieveData();
  }

  // Get data if login before
  _retrieveData = async () => {
    try {
      const _user = await AsyncStorage.getItem('USER_SSC');
      const _password = await AsyncStorage.getItem('PASSWORD_SSC');

      if (!!_user && !!_password) {
        // Call API
        this.props.fetchDataLogin(_user, _password)
          .then(() => setTimeout(() => {
            this.showAlert()
          }, 100));
      }
      else {
        setTimeout(() => {
          this.props.navigation.navigate("Login");
        }, 1000)
      }
    } catch (error) {
      // Error retrieving data
    }
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
      if (dataRes.hasError == true) {
        setTimeout(() => {
          this.props.navigation.navigate("Login");
        }, 500)
      } else if (dataRes.hasError == false) {
        setTimeout(() => {
          this.props.navigation.navigate("Home");
        }, 500);
      }
    }
  }

  // Render
  render() {
    return (
      <View>
        <LinearGradient
          start={{ x: 0, y: 1 }}
          end={{ x: 0, y: -1 }}
          locations={[0.0, 0.7, 1.0]}
          colors={[COLORS.BLUE_2F6F7A, COLORS.BLUE_2E5665]}>
          <View style={styles.container}>
            <Image style={styles.logo} source={LOGO} />
            <Text style={styles.text}>{STRINGS.WelcomeText}</Text>
            <ActivityIndicator size="small" color={COLORS.WHITE_FFFFFF} />
          </View>
        </LinearGradient>
      </View>
    );
  }
}

const styles = StyleSheet.create({
  container: {
    height: height,
    justifyContent: 'center',
    alignItems: 'center'
  },
  text: {
    marginTop: 20,
    marginBottom: 20,
    fontSize: 20,
    color: COLORS.WHITE_FFFFFF,
    textAlign: 'center',
    fontFamily: FONTS.MAIN_FONT_BOLD
  },
  logo: {
    opacity: 1,
    width: 100,
    height: 100
  }
})

function mapStateToProps(state) {
  return {
    isLoading: state.loginReducer.isLoading,
    dataRes: state.loginReducer.dataRes,
    error: state.loginReducer.error,
    errorMessage: state.loginReducer.errorMessage,
  }
}

function dispatchToProps(dispatch) {
  return bindActionCreators({
    fetchDataLogin
  }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(Welcome);