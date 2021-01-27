<template>
  <div id="app">
    <van-nav-bar title="陈医生"
                 left-text="返回"
                 left-arrow
                 @click-left="onClickLeft" />
    <router-view />
  </div>
</template>

<script>
export default {
  name: 'App',
  methods: {
    onClickLeft () {
      alert('返回上一级')
    }
  }
}
</script>

<style>
#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #c2c2c2;
  margin-top: 50px;
  margin: 0 auto;
  width: 500px;
}
</style>
