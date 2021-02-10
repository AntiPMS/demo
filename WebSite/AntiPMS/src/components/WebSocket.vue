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
         style="overflow-y: scroll">
      <van-cell v-for="(msgtext,index) in receiveMsgText"
                :key="msgtext.msg">
        <!-- 别人的消息 Start -->
        <transition name="van-slide-up">
          <a v-if="msgtext.senderId!=sendMsg.senderId">
            <img src="../assets/医生头像.png"
                 style="float: left;margin-top: 15px">
            <div style="float: left">
              <label style="color:#3f9775">{{msgtext.senderName}}</label>
              <br>
              <!-- 文本消息 -->
              <div v-if="msgtext.msgType==1">
                <input :value=msgtext.msg
                       style=" border-radius: 5px;color:#3f9775;border: 1px solid #dadada;padding-left: 5px;font-weight: 100"
                       :style="{ width:msgtext.msg.length*16 + 'px' }"
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
            <img src="../assets/医生头像.png"
                 style="float: right;margin-top: 15px">
            <div style="float: right">
              <label style="float: right;padding-right: 5px;color:#3f9775">{{msgtext.senderName}}</label>
              <br>
              <!-- 文本消息 -->
              <div v-if="msgtext.msgType==1">
                <input :value=msgtext.msg
                       style=" border-radius: 5px;color:#3f9775;border: 1px solid #dadada;padding-left: 5px;font-weight: 100"
                       :style="{ width:msgtext.msg.length*16 + 'px' }"
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
        </transition>
        <!-- 自己的消息 End -->
      </van-cell>
    </div>
    <!-- 消息主体 End-->
    <!-- 底部工具、输入框 Start -->
    <div ref="container"
         style="bottom:0px">
      <van-cell-group>
        <van-cell>
          <van-row>
            <van-col>
              <van-field v-model="sendMsg.msg"
                         v-on:keyup.enter="doSend(true)"
                         type="input"
                         maxlength="1000"
                         style="float: left;border:5px;border-color:black;"
                         :style="width" />
            </van-col>
            <van-button v-on:click="doSend(true)"
                        type="primary"
                        style="float:left;margin-left: 15px">发送</van-button>
          </van-row>
        </van-cell>
      </van-cell-group>
    </div>
    <!-- 底部工具、输入框 End -->
  </div>
</template>
<style scoped>
.van-field__control {
  border: 2px solid green;
}
.van-cell {
  width: 95%;
  padding: 0;
}
.van-cell::after {
  border: 0;
}
.van-cell__value {
  padding-left: 5%;
}
</style>
<style>
input:disabled {
  cursor: default;
  background-color: rgba(255, 255, 255, 0.3);
}
</style>
<script>
export default {
  name: 'MyWebSocket',
  data () {
    return {
      currentMsg: '',
      showMsg: false,
      container: null,
      socket: null,
      receiveMsgText: [],
      imgWidth: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], // 要设置的img宽
      imgHeigth: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], // 要设置的img高
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
    this.sendMsg.senderId = this.guid() // 'qinko'
    this.sendMsg.senderName = this.sendMsg.senderId// this.guid().substring(4)
    this.sendMsg.targetId = '233'
    this.hh()
    this.doConnect(this)
  },
  mouted: function () {
    this.container = this.$refs.container
  },
  beforeDestroy: function () {

  },
  destroy: function () {
    this.socket.onclose()
  },
  methods: {
    hh () {
      this.height.height = document.documentElement.clientHeight - 80 + 'px'
      this.width.width = document.documentElement.clientWidth - 120 + 'px'
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
      let uri = 'ws://cloudhospital.knjs.com.cn:1071/ws?senderId=' + this.sendMsg.senderId + '&targetId=' + this.sendMsg.targetId
      this.socket = new WebSocket(uri)
      let showNotify = this.showNotify
      this.socket.onopen = function (e) {

      }
      this.socket.onclose = function (e) {

      }
      this.socket.onmessage = function (e) {
        var rmsg = JSON.parse(e.data)
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
    },
    onload (e) { // 图片加载时
      let imgW = e.target.naturalWidth// 图片宽
      let imgH = e.target.naturalHeight // 图片高
      let index = e.target.id
      // 如果图片宽高小于浏览器,不做改变
      if (imgW < document.documentElement.clientWidth) {
        this.imgWidth[index] = imgW
        this.imgHeigth[index] = imgH
      } else {
        // 图片的宽高比例大于dialog最大宽高比时，以dialog内容的最大宽度为图片的宽度
        this.imgWidth[index] = document.documentElement.clientWidth - 50
        this.imgHeigth[index] = (document.documentElement.clientWidth - 50) * imgH / imgW
      }
    }
  }
}

</script>
