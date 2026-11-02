const amqp = require('amqplib');

async function setup() {
    
    const connection = await amqp.connect('amqp://root:Mudar@2020@localhost');

    const channel = await connection.createChannel();

    let mensagem = { nome: 'wellington', email: 'wellingtonbr@gmail.com', telefone: '+5511981552010' };

    for (let i = 0, j = 1_000 * 10; i < j; i++)
        //channel.publish('contato_exchange', 'novo.contato', Buffer.from(JSON.stringify(mensagem)), { persistent: true });
        channel.publish('contato_exchange', 'some.key', Buffer.from(JSON.stringify(mensagem)), { persistent: true });

    console.log('mensagem publicada com sucesso');

    await channel.close();
    
    await connection.close();
}

setup().catch(console.error);