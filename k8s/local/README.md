# FCG.Catalog local Kubernetes

Esta pasta contem somente os manifests locais da aplicacao `fcg-catalog`.

A infra compartilhada fica em:

```text
fcg-orchestration/fase-04/k8s
```

Suba a infra primeiro:

```bash
cd fcg-orchestration/fase-04/k8s
bash up.sh
```

Para recriar apenas o `fcg-catalog`:

```bash
cd fcg-catalog
bash k8s/local/up.sh
```

Para remover apenas o namespace da aplicacao:

```bash
bash k8s/local/down.sh
```

Comandos uteis:

```bash
kubectl get pods -n fcg-catalog
kubectl logs -n fcg-catalog deployment/fcg-catalog -f
kubectl describe pod -n fcg-catalog -l app.kubernetes.io/name=fcg-catalog
curl http://localhost:5000/health
```
