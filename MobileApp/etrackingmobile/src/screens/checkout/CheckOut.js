import React, { Component } from 'react';
import { View, AsyncStorage, ActivityIndicator, Alert, Dimensions, StyleSheet, Image } from "react-native";
import { Text } from 'native-base';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";

class CheckOut extends Component {

  constructor(props) {
    super(props);
  }

  handleBack = () => {
    this.props.navigation.navigate('Home');
  }

  handleCheckOut = () => {
    this.props.navigation.navigate('Home');
  }

  render() {
    return (
      <View
        style={styles.container}>
        <MainHeader
          onPress={() => this.handleBack()}
          hasLeft={true}
          title={STRINGS.CheckOutTitle} />
        <View
          padder
          style={styles.subContainer}>
          <Text>{STRINGS.CheckOutSubTitle}</Text>
          <Text style={styles.text}>{'8:00'}</Text>
          <MainButton
            style={styles.button}
            title={STRINGS.CheckOutAction}
            onPress={() => this.handleCheckOut()} />
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
    fontSize: 40,
    color: 'red',
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

  }
}

function dispatchToProps(dispatch) {
  return bindActionCreators({
  }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(CheckOut);