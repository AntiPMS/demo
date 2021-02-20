<template>
  <div>
    <!-- 消息通知 Start-->
    <van-notify v-model="showMsg"
                type="success">
      <van-icon name="bell"
                style="margin-right: 4px;" />
      <span>{{currentMsg}}</span>
    </van-notify>
    <!-- 消息通知 End-->
    <!-- 消息主体 Start-->
    <div :style="height"
         style="overflow-y: scroll"
         id="dhk">
      <van-cell v-for="(msgtext,index) in receiveMsgText"
                :key="msgtext.msg">
        <!-- 别人的消息 Start -->
        <a v-if="msgtext.senderId!=sendMsg.senderId">
          <img src="../assets/医生头像@3x.png"
               style="float: left;margin-top: 15px;width:44px;height: 44px">
          <div style="float: left">
            <label style="color:#3f9775;font-size: 12px;">{{msgtext.senderName}}</label>
            <br>
            <!-- 文本消息 -->
            <div v-if="msgtext.msgType==1">
              <input :value=msgtext.msg
                     style=" border-radius: 5px;color:black;border: 1px solid #dadada;padding-left: 5px;font-weight: 100;width:auto;"
                     disabled>
            </div>
            <div v-else-if="msgtext.msgType==2">
              <!-- 图片 -->
              <img :src=msgtext.msg
                   :style="{width: imgWidth[index] + 'px', height: imgHeigth[index] + 'px'}"
                   :id=index
                   @load="onload">
            </div>
          </div>
        </a>
        <!-- 别人的消息 End -->
        <!-- 自己的消息 Start-->
        <a v-else-if="msgtext.senderId==sendMsg.senderId">
          <img src="../assets/医生头像@3x.png"
               style="float: right;margin-top: 15px;width:44px;height: 44px">
          <label style="float: right;padding-right: 5px;color:#3f9775;font-size: 12px">{{msgtext.senderName}}</label>
          <br>
          <div style="float: right">
            <!-- 文本消息 -->
            <div v-if="msgtext.msgType==1">
              <input :value=msgtext.msg
                     style=" border-radius: 5px;color:black;border: 1px solid #dadada;padding-left: 5px;font-weight: 100;width:auto;"
                     disabled>
            </div>
            <div v-else-if="msgtext.msgType==2">
              <!-- 图片 -->
              <label style="float: right;padding-right: 5px;color:#3f9775;font-size: 12px">{{msgtext.senderName}}</label>
              <br>
              <img :src=msgtext.msg
                   :style="{width: imgWidth[index] + 'px', height: imgHeigth[index] + 'px'}"
                   :id=index>
            </div>
          </div>
        </a>
        <!-- 自己的消息 End -->
      </van-cell>
    </div>
    <!-- 消息主体 End-->
    <!-- 底部工具、输入框 Start -->
    <div style="margin-bottom:5px;margin-top: -1px">
      <van-field v-model="sendMsg.msg"
                 v-on:keyup.enter="doSend(true)"
                 type="input"
                 maxlength="1000"
                 class="communicationInput"
                 style="float: left;"
                 :style="{width:KD}" />
      <van-button v-on:click="doSend(true)"
                  type="primary"
                  style="float:right;margin-right: 10px;margin-top: 5px">发送</van-button>
    </div>
    <!-- 底部工具、输入框 End -->
  </div>
</template>
<script>
export default {
  name: 'MyWebSocket',
  data () {
    return {
      KD: '',
      currentMsg: '',
      showMsg: false,
      socket: null,
      receiveMsgText: [],
      imgWidth: [100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100], // 要设置的img宽
      imgHeigth: [100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100], // 要设置的img高
      height: {
        height: ''
      },
      width: {
        width: ''
      },
      sendMsg: {
        Id: '',
        senderId: '',
        senderName: '',
        targetId: '',
        msgType: 1,
        msg: ''
      }
    }
  },
  created: function () {
    // init sendMsg object
    this.sendMsg.senderId = 'asdddd' // 'qinko'
    this.sendMsg.senderName = this.sendMsg.senderId// this.guid().substring(4)
    this.sendMsg.targetId = '5'
    this.getLenth()
    this.doConnect(this)
  },
  mouted: function () {

  },
  beforeDestroy: function () {

  },
  destroy: function () {
    this.socket.onclose()
  },
  methods: {
    getLenth () {
      this.height.height = document.documentElement.clientHeight - 80 + 'px'
      this.width.width = document.documentElement.clientWidth - 120 + 'px'
      this.KD = document.documentElement.clientWidth - 120 + 'px'
    },
    upLoadFile (file) {
      this.sendMsg.msgType = 2
      this.sendMsg.msg = file.content
      this.socket.send(JSON.stringify(this.sendMsg))
      this.sendMsg.msg = ''
    },
    showNotify (msg) {
      this.showMsg = true
      this.currentMsg = msg
      setTimeout(() => {
        this.showMsg = false
      }, 2000)
    },
    guid () {
      function S4 () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1)
      }
      return (S4() + S4())
    },
    doConnect (app) {
      // cloudhospital.knjs.com.cn:1071
      // api.qinko.club
      let uri = 'wss://api.qinko.club/ws?senderId=' + this.sendMsg.senderId + '&targetId=' + this.sendMsg.targetId
      this.socket = new WebSocket(uri)
      let showNotify = this.showNotify
      this.socket.onopen = function (e) {

      }
      this.socket.onclose = function (e) {

      }
      this.socket.onmessage = function (e) {
        var rmsg = JSON.parse(e.data)
        if (rmsg['MsgType'].toString() === '6') {

        } else {
          app.receiveMsgText.push(
            {
              Id: rmsg['Id'],
              senderId: rmsg['SenderId'],
              senderName: rmsg['SenderName'],
              msgType: rmsg['MsgType'],
              msg: rmsg['Msg']
            })
          showNotify(rmsg['Msg'])
        }
      }
      this.socket.onerror = function (e) {
        app.receiveMsgText.push({
          Id: this.guid(),
          senderId: 'system',
          senderName: 'system',
          msgType: '1',
          msg: '连接已断开'
        })
      }
    },
    doSend () {
      this.sendMsg.msgType = 1
      this.socket.send(JSON.stringify(this.sendMsg))
      this.receiveMsgText.push(
        {
          Id: (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1),
          senderId: this.sendMsg.senderId,
          senderName: this.sendMsg.senderName,
          msgType: 1,
          msg: this.sendMsg.msg
        })
      this.sendMsg.msg = ''
      var div = document.getElementById('dhk')
      div.scrollTop = div.scrollHeight - 200
    },
    onload (e) { // 图片加载时
      let imgW = e.target.naturalWidth// 图片宽
      let imgH = e.target.naturalHeight // 图片高
      let index = e.target.id
      if (imgW < document.documentElement.clientWidth) {
        this.imgWidth[index] = imgW
        this.imgHeigth[index] = imgH
      } else {
        // 图片的宽高比例大于dialog最大宽高比时，以dialog内容的最大宽度为图片的宽度
        this.imgWidth[index] = document.documentElement.clientWidth - 150
        this.imgHeigth[index] = (document.documentElement.clientWidth - 150) * imgH / imgW
      }
    },
    websocketHeartCheck () {
      this.socket.send(
        JSON.stringify({
          Id: '',
          senderId: this.sendMsg.senderId,
          senderName: this.sendMsg.senderName,
          targetId: this.targetId,
          msgType: 6,
          msg: ''
        }))
    }
  }
}

</script>
<style>
.communicationInput input {
  border: 2px solid #3f9775;
}
</style>
