import React, { Component } from "react";
import { View, Alert, AsyncStorage, StyleSheet } from "react-native";
import { Container, Input, Item, Text } from "native-base";
import Toast from 'react-native-root-toast';
import Spinner from 'react-native-loading-spinner-overlay';
import { KeyboardAwareScrollView } from 'react-native-keyboard-aware-scroll-view';
import { MainButton } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import { fetchDataLogin } from '../../redux/actions/ActionLogin';

class Login extends Component {

  constructor(props) {
    super(props);
    this.state = {
      Email: '',
      Password: '',
      Message: '',
      isVisible: false
    }
  }

  reset() {
    this.setState({
      Email: '',
      Password: '',
      Message: '',
      isVisible: false
    });
  }

  _storeData = async (_user, _password) => {
    try {
      await AsyncStorage.setItem('USER_SSC', _user);
      await AsyncStorage.setItem('PASSWORD_SSC', _password);
    } catch (error) {
    }
  }

  handleLogin() {

    let _email = this.state.Email;
    let _password = this.state.Password;

    // Check validate
    if (_email == '') {
      this.setState({ isVisible: true, Message: STRINGS.LoginEmailEmpty });
      setTimeout(() => this.setState({
        isVisible: false
      }), 2000);
      return;
    }

    if (_password == '') {
      this.setState({ isVisible: true, Message: STRINGS.LoginPasswordEmpty });
      setTimeout(() => this.setState({
        isVisible: false
      }), 2000);
      return;
    }

    // Save user info on local 
    this._storeData(_email, _password);

    // Call API
    this.props.fetchDataLogin(_email, _password)
      .then(() => setTimeout(() => {
        this.showAlert()
      }, 100));
  }

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
        this.reset();
        this.props.navigation.navigate("Home");
      }
    }
  }

  renderMessage() {
    return (
      <Toast
        visible={this.state.isVisible}
        position={50}
        shadow={false}
        animation={false}
        hideOnPress={true}>
        {this.state.Message}
      </Toast>
    );
  }

  render() {
    const { Email, Password } = this.state;
    const { isLoading } = this.props;
    return (
      <Container>
        {this.renderMessage()}
        <Spinner visible={isLoading} />

        <KeyboardAwareScrollView
          enableAutomaticScroll={true}
          scrollEnabled={true}
          contentContainerStyle={styles.keyboardContainer}>
          <View padder style={styles.container}>
            <Text
              style={styles.title}
              uppercase>
              {STRINGS.LoginTitle}
            </Text>
            <Item regular style={styles.item}>
              <Input
                style={styles.input}
                placeholder={STRINGS.LoginEmail}
                onChangeText={text => this.setState({ Email: text })}>
                {Email}
              </Input>
            </Item>
            <Item regular style={styles.item}>
              <Input
                secureTextEntry={true}
                style={styles.input}
                placeholder={STRINGS.LoginPassword}
                onChangeText={text => this.setState({ Password: text })}>
                {Password}
              </Input>
            </Item>
            <MainButton title={STRINGS.LoginAction} onPress={() => this.handleLogin()}></MainButton>
            <Text style={{ marginTop: 30 }}>
              {'v3.0'}
            </Text>
          </View>
        </KeyboardAwareScrollView>

      </Container>
    );
  }
}

const styles = StyleSheet.create({
  keyboardContainer: {
    flexGrow: 1,
    justifyContent: 'center',
    alignItems: 'center'
  },
  container: {
    flex: 1,
    flexDirection: 'column',
    justifyContent: 'center',
    alignItems: 'center',
    maxWidth: 500,
    padding: 30
  },
  logo: {
    marginBottom: 50,
    width: 100,
    height: 100
  },
  title: {
    fontSize: 20,
    fontFamily: FONTS.MAIN_FONT_BOLD,
    marginBottom: 50,
    color: COLORS.BLUE_2E5665
  },
  item: {
    height: 40,
    marginBottom: 20
  },
  input: {
    fontFamily: FONTS.MAIN_FONT_REGULAR
  },
});

function mapStateToProps(state) {
  return {
    isLoading: state.loginReducer.isLoading,
    dataRes: state.loginReducer.dataRes,
    error: state.loginReducer.error,
    errorMessage: state.loginReducer.errorMessage
  }
}

function dispatchToProps(dispatch) {
  return bindActionCreators({
    fetchDataLogin
  }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(Login);
