# GoodTiger

c# Socket server.

It started for hu, but it's open to everyone.

If you ask questions or requests through Kakao Talk or issues, we'll review them and proceed.

Contact kakao https://open.kakao.com/o/g66zPHD

안녕하세요.

굿타이거 개발자입니다.

얼마 전에 GC 조정까지 끝냈고요 

프로토콜 부분만 개인화 하시면 

프로젝트에 잘 써먹을 수 있을 거 같습니다.

조금이라도 도움이 되면 좋겠습니다.

## Change Port

If you want to change the port, Change the value in appsetting.json:Port.

## Environmental variables

Add the appsetting.{type}.json and pass the entered {type} value to the ENVIRONMENT variable.

## Performance indicators

I measured Request Per Second.

It's not as fast as C++, but it's not bad.

![](https://github.com/fatherscott/GoodTiger/blob/main/image/RPS.PNG)

## GC Tuning

Checked that 2nd generation gc is not occurring

![](https://github.com/fatherscott/GoodTiger/blob/main/image/gc_tuning.PNG)
