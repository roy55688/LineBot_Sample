# LineBot_Sample
Built with the Line.Messaging.NETCore package, the LineBot can parse links containing images from Pixiv and Twitter and convert them into Bubble Messages.

# LineBot�ϥΤ�k
1. �ݥ��� https://developers.line.biz/zh-hant/ �إ� MessageAPI�C
1. �N�ӽШ쪺 AccessToken �P ChannelSecret ��J�{���ϥΡC(���d�ҬO��J�����ܼƨϥ�)
1. ���a�ݴ��եi�ϥ� ngrok ���{���N�}�o���Ҫ��s�u��~�H�]�wWebhook URL�C
1. �] LineBot �� Webhook URL �T�w�A�]�����d�Ҩϥ� minimal API �Ӻ޲z���ѡA�ñN�y�{����h���m Application�C
1. ���d�ҨϥΪ��M�� https://www.nuget.org/packages/Line.Messaging.NETCore/


## TwitterService
1. ���d�ҽեβĤT�� API �ӸѪR tweet ���Ϥ��Ȭ����ըϥΡA�ФŪ����ϥΦb�Ͳ����ҡA�Ψ��o API ���Ѫ̪����v�C
1. TwitterService �L��L�B�~�ѼƳ]�w�A�i�����i����աC
1. �ϥνd�Ҧp�U :
![image](https://github.com/roy55688/LineBot_Sample/blob/main/LineBot_Sample_twitter.jpg?raw=true)

## PixivService
1. ���d�Ҩϥ� cookie �Ӽ����n�J�̨����A�ñN�Ϥ��W�Ǧ� Imgur ��H���o�Ϥ� url �ӻs�@ Line �^�_�T���A�]���ݭn�]�w�H�U�ѼơC
1. �n�J cookie �i�H��n�J pixiv �����ɱq�s�����W���o�C
1. Imgur �ݵ��U�åӽ� Client ID �ϥΡC
1. �ϥνd�Ҧp�U :
![image](https://github.com/roy55688/LineBot_Sample/blob/main/LineBot_Sample_pixiv.jpg?raw=true)

### ���d�үº�H�ǳN��y�P�ӤH��@�m�߬��D�A�Y�����󤣧����pô���H�i�沾���C