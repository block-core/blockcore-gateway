# Blockcore Platform

> The Blockcore Platform is a distributed system that integrates with Blockcore Blockchain technologies.

The Blockcore Platform can be used to build distributed apps and systems, that store data and metadata off-chain, but utilize the blockchain for data integrity, data validation and signing.

## Functional Background

The functional requirement for developing the Blockcore Platform, is to have a distributed network for exchanging data securely, but that keeps data off the main blockchain.

Example on usage is a payment scenario, where a user scans a payment request by another individual, or a merchant, and the payment app can retrieve data (metadata, including company logo or profile photo) about the merchant/individual from the Blockcore Platform. This data can be used to add trust and identification on the validitity of a payment request.

Without this data and validation, there are many possible attack-vectors in a payment scenario, for example the QR-code for payment can be manipulated on the receiver device, and the payer has zero ability to verify that the payment address.

Other examples is identities and metadata attached to identities. Metadata must be open for edited and withdrawable by the individual who owns the identity.

## Blockcore Identity

The Blockcore Platform relies heavily on the Blockcore Identity Framework.

[BIP 32](https://github.com/bitcoin/bips/blob/master/bip-0032.mediawiki)

```
m / purpose' / coin_type' / account' / change / address_index
```

<img src="https://raw.githubusercontent.com/bitcoin/bips/master/bip-0032/derivation.png"></img>

The purpose field: [BIP 43](https://github.com/bitcoin/bips/blob/master/bip-0043.mediawiki)

> "We encourage different schemes to apply for assigning a separate BIP number and use the same number for purpose field, so addresses won't be generated from overlapping BIP32 spaces."

Registered coins: [BIP 44](https://github.com/satoshilabs/slips/blob/master/slip-0044.md)

- Identities does not require any metadata and can simply me recognized as public key (address derived from public key).
- Users can have any number of identities, that are derived from a custom path in their existing HD-wallet.
- Should HD paths be similar across Blockcore-base blockchains and result in different visible addresses, but have the same private and public key values, or should they be entirely different with different paths?
- Should purpose be used?
- m/44'/60'/0'/0

DIF: https://identity.foundation/   
DID: https://w3c-ccg.github.io/did-spec/

## Blockcore Networking

The Blockcore Platform relies on the Blockcore Networking Framework to perform true peer-to-peer networking without relying on firewall port opening, and has high-performance and encrypted data-transfer between participants in the network. TODO: Research into WebRTC and the possibility of supporting WebRTC in the full node.

## Payment Requests

When a Point-of-Sale terminal or any user of the Blockcore-compatible wallets presents a payment request, they can decide if the payment request should be anonymous or share identity (public key) and encryption key (optional).

To avoid putting technical information into the *Label* and *Message* values, which most modern wallets will be able to read and display to a user, we must rely on a custom parameter for the identity. This means a payment will be performed and displayed normally on any third party wallet, but the Blockcore-compatible wallets can potentially display additional metadata about the merchant/receiver.

```C#
NBitcoin.Payment.BitcoinUrlBuilder uri = new NBitcoin.Payment.BitcoinUrlBuilder();
uri.UnknowParameters.Add("identity", "Ccqvt3HZBd9M8r3RyuR89Wbt4j");
uri.UnknowParameters.Add("identity-key", "XXX"); // Optional
uri.Amount = new NBitcoin.Money((long)20000);
uri.Label = "Average Joe Coffee Shop";
uri.Message = "Cup of coffee";
uri.Address = NBitcoin.BitcoinAddress.Create("bc1q3dmr8mtycf3ummrh84mye85ueu87ussllkc0v3", NBitcoin.Network.Main);
var url = uri.ToString();
// url = bitcoin:bc1q3dmr8mtycf3ummrh84mye85ueu87ussllkc0v3?amount=0.0002&label=Average%20Joe%20Coffee%20Shop&message=Cup%20of%20coffee&identity=Ccqvt3HZBd9M8r3RyuR89Wbt4j&identity-key=XXX
```

This address can be encoded into an QR code for quick scanning, and is based on the [BIP 21 specification](https://github.com/bitcoin/bips/blob/master/bip-0021.mediawiki).

### Considerations

- Should the identity be given in clear-text, or should it be unique per payment and require decryption (address+identity=key)?
- The identity-key is an encryption key that opens up private meta-data of the receiver. This is optional.
- When a user performs a payment, should they persist the identity and identity-key on their private vault on the Blockcore Platform? Unless they do, they either have to store the metadata at the point of payment performed, or they will be unable to restore (during a wallet restore) a full transaction history in the future.
- Storing the full history makes it hard for others to withdraw metadata, so really only the identity (and perhaps identity-key) should be stored in the platform. This means in the future, a merchant/individual can withdraw all metadata and when a wallet is restored, the history information will disappear (not be available).
- Receiving the encoded payment request URI should additionally support NFC (Near-Field-Communication). NFC support enables PoS (Point-of-Sale) setup where no screen is needed.
- Data retrieved from the platform, can be integrity-validated to ensure it is untamprered.

## Payment for services

When a user utilized the services provided by the participants in the distributed nodes network, they will be performing payment to all members of the network. These payments are automatic, and users will need to have wallets with balance to perform (automatic) payment for services utilized.

- Users running platform nodes should receive payment for utilizing storage.
- What distribution model makes sense for the platform nodes?

## Communities

The platform should enable encrypted and fully anonymous secure communities. Enabling anyone, friends, neighbourhoods, merchants or anyone to communicate secure and anonymous. The same identity used for payments, can also be used for communities.

A community owner should be able to decide if to utilize the platform nodes for persistent storage of encrypted community content, or if the platform nodes should only be used for message relay.

The community will largely be built as a client-side implementation, with persistent storage in the web browser.

## Sidechain Metadata

Platform should have the ability to store and shared metadata related to sidechains that runs on the blockchain. This is useful to have additional information available, that can securely verified and updated by owners of sidechains.

## Contracts

Platform should be able to store contracts, both in form of analog documents and semi-smart contracts.

## Subscriptions

Platform should support subscriptions, with the ability for auto-withdrawal from multi-sig wallets that are manually or automatically populated from regular wallets.

## Point-of-Sale

Platform must support the ability to store Point-of-Sale databases for merchants. Merchants must pay for this service, but it will enable merchants to have a fully distributed network of PoS-devices that retrieve the latest database from the platform nodes.

## Inter-Chain Links

The platform should be able to keep inter-chain links connected to the identities on the platform. This means connecting the ownership or public key on one blockchain with the identities that exists on the platform.

This can be used to verify ownership of cars on a "car-blockchain" (https://vinchain.io/, https://www.carvertical.com/en/), or insurance policy on an "insurance-blockchain". Could also be used to connect sidechains on a Blockcore-based blockchain, with identity that exists on the platform.

This type of information can be deployed on the Blockcore Platform Portal, where a user can see all their connected assets across multiple blockchains.

## Attributions

Thanks to [Benjamin Watkins](https://github.com/7wingfly/P2Pchat/) for providing a workable proof-of-concept for P2P communication in C#.

## License

Blockore Platform by Blockore, licensed as MIT
Parts by Benjamin Watkins, licensed as MIT